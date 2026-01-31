using System.Diagnostics;
using System.Threading.Channels;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PIED_LMS.Application.Abstractions;
using PIED_LMS.Contract.Services.Compiler.Requests;
using PIED_LMS.Contract.Services.Compiler.Responses;

namespace PIED_LMS.Infrastructure.Services.Compiler;

public class DockerCompilerService : ICompilerService, IHostedService, IDisposable
{
    private const string DockerImage = "gcc:latest"; // Adjust as needed
    private const int PoolSize = 5;
    private const string ContainerNamePrefix = "pied_compiler";
    private const string CompileSuccessMarker = "COMPILE_SUCCESS_MARKER";

    // Configurations
    private const long ContainerMemory = 100 * 1024 * 1024; // 100MB
    private const long ContainerNanoCpus = 1000000000; // 1 CPU
    private const long PidsLimit = 50;
    private readonly DockerClient _client;
    private readonly Channel<string> _containerPool;
    private readonly ILogger<DockerCompilerService> _logger;

    public DockerCompilerService(ILogger<DockerCompilerService> logger)
    {
        _logger = logger;
        _client = new DockerClientConfiguration().CreateClient();
        _containerPool = Channel.CreateBounded<string>(PoolSize);
    }

    public async Task<CompileResponse> CompileAsync(CompileRequest request, string identifier,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement rate limiting using identifier
        _logger.LogDebug("Compile request from identifier: {Identifier}", identifier);

        var container = await _containerPool.Reader.ReadAsync(cancellationToken);
        try
        {
            return await ExecuteCompileAsync(container, request, cancellationToken);
        }
        finally
        {
            await _containerPool.Writer.WriteAsync(container, cancellationToken);
        }
    }

    public async Task<JudgeResult> JudgeAsync(JudgeRequest request, string identifier, CancellationToken cancellationToken = default)
    {
        // TODO: Implement rate limiting using identifier
        _logger.LogDebug("Judge request from identifier: {Identifier}", identifier);

        var container = await _containerPool.Reader.ReadAsync(cancellationToken);
        try
        {
            return await ExecuteJudgeAsync(container, request, cancellationToken);
        }
        finally
        {
            await _containerPool.Writer.WriteAsync(container, cancellationToken);
        }
    }

    public void Dispose() => _client.Dispose();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Initializing Compiler Container Pool...");

        // Ensure image exists
        await EnsureImageExistsAsync();

        for (var i = 0; i < PoolSize; i++)
        {
            var containerName = $"{ContainerNamePrefix}_{i}";
            await EnsureContainerRunningAsync(containerName);
            await _containerPool.Writer.WriteAsync(containerName, cancellationToken);
        }

        _logger.LogInformation("Compiler Container Pool Initialized.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Optional: Stop containers on shutdown? 
        // For now, we leave them running for faster restart, or we can clean them up.
        // Given the requirement "StopContainer" in TS, maybe we should stop them?
        // TS has stopContainer method. Let's try to be nice.
        // But stopping 5 containers might take time.
        return Task.CompletedTask;
    }

    private async Task EnsureImageExistsAsync()
    {
        var images = await _client.Images.ListImagesAsync(new ImagesListParameters { All = true });
        if (!images.Any(i => i.RepoTags != null && i.RepoTags.Contains(DockerImage + ":latest")))
        {
            _logger.LogInformation($"Pulling Docker image {DockerImage}...");
            await _client.Images.CreateImageAsync(
                new ImagesCreateParameters { FromImage = DockerImage, Tag = "latest" },
                null,
                new Progress<JSONMessage>());
        }
    }

    private async Task EnsureContainerRunningAsync(string containerName)
    {
        try
        {
            var containers = await _client.Containers.ListContainersAsync(new ContainersListParameters { All = true });
            var existing = containers.FirstOrDefault(c => c.Names.Contains("/" + containerName));

            if (existing != null)
            {
                if (existing.State == "running")
                {
                    await FixWorkspacePermissionsAsync(existing.ID);
                    return;
                }

                await _client.Containers.RemoveContainerAsync(existing.ID,
                    new ContainerRemoveParameters { Force = true });
            }

            var config = new CreateContainerParameters
            {
                Name = containerName,
                Image = DockerImage,
                HostConfig = new HostConfig
                {
                    NetworkMode = "none",
                    Memory = ContainerMemory,
                    NanoCPUs = ContainerNanoCpus,
                    PidsLimit = PidsLimit,
                    CapDrop = new List<string> { "ALL" },
                    ReadonlyRootfs = true,
                    Tmpfs = new Dictionary<string, string> { { "/workspace", "rw,nodev,nosuid,size=100m,exec" } },
                    SecurityOpt = new List<string> { "no-new-privileges" }
                },
                WorkingDir = "/workspace",
                Cmd = new List<string> { "sleep", "infinity" },
                User = "root" // Start as root, drop to nobody for execution
            };

            var response = await _client.Containers.CreateContainerAsync(config);
            await _client.Containers.StartContainerAsync(containerName, new ContainerStartParameters());
            await FixWorkspacePermissionsAsync(response.ID);

            _logger.LogInformation($"Started container {containerName}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to start container {containerName}");
            throw;
        }
    }

    private async Task FixWorkspacePermissionsAsync(string containerId)
    {
        try
        {
            // chmod 777 /workspace to allow 'nobody' to write
            var cmd = new List<string> { "chmod", "777", "/workspace" };
            var execCreate = await _client.Exec.ExecCreateContainerAsync(containerId, new ContainerExecCreateParameters
            {
                Cmd = cmd,
                User = "root"
            });
            await _client.Exec.StartAndAttachContainerExecAsync(execCreate.ID, false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Failed to fix permissions for container {containerId}");
        }
    }

    private async Task<CompileResponse> ExecuteCompileAsync(string container, CompileRequest request,
        CancellationToken ct)
    {
        var workspaceId = Guid.NewGuid().ToString("N");
        var sourceFile = $"code_{workspaceId}.c";
        var exeFile = $"code_{workspaceId}";

        var encodedCode = Convert.ToBase64String(Encoding.UTF8.GetBytes(request.Code));
        var gccFlags = $"-O{request.OptimizationLevel} -std=c11"; // Add flags

        long compilationTime = 0;
        long executionTime = 0;

        try
        {
            // 1. Compile
            var sw = Stopwatch.StartNew();

            var compileCmd = new List<string>
            {
                "sh", "-c",
                $"echo \"{encodedCode}\" | base64 -d > {sourceFile} && " +
                $"gcc {sourceFile} -o {exeFile} {gccFlags} 2>&1"
            };

            var (compileStdout, compileStderr, compileExitCode) =
                await ExecuteSimpleCommandAsync(container, compileCmd, ct);
            sw.Stop();
            compilationTime = sw.ElapsedMilliseconds;

            // Likely compile error if non-zero exit code or explicit error output
            if (compileExitCode != 0 || (compileStdout + compileStderr).Contains("error:"))
            {
                _ = CleanupFilesAsync(container, sourceFile);
                var error = (compileStdout + compileStderr).Trim();
                return new CompileResponse(false, Error: error, CompilationTime: compilationTime);
            }

            // 2. Run
            var runCmdList = new List<string>();
            var runCmdString = $"timeout {Math.Ceiling(request.TimeLimit / 1000.0)}s ./{exeFile}";

            if (!string.IsNullOrEmpty(request.Input))
            {
                var encodedInput = Convert.ToBase64String(Encoding.UTF8.GetBytes(request.Input));
                // echo "base64" | base64 -d | ./program
                // Note: We need to ensure we are running in sh
                runCmdList.Add("sh");
                runCmdList.Add("-c");
                runCmdList.Add($"echo \"{encodedInput}\" | base64 -d | {runCmdString}");
            }
            else
            {
                runCmdList.Add("sh");
                runCmdList.Add("-c");
                runCmdList.Add(runCmdString);
            }

            sw.Restart();
            var (runStdout, runStderr, runExitCode) = await ExecuteSimpleCommandAsync(container, runCmdList, ct);
            sw.Stop();
            executionTime = sw.ElapsedMilliseconds;

            _ = CleanupFilesAsync(container, sourceFile, exeFile);

            var output = runStdout;
            var errorOut = runStderr;

            // Basic Error Detection
            if (errorOut.Contains("Segmentation fault") || output.Contains("Segmentation fault"))
                return new CompileResponse(false, Error: "Segmentation fault", CompilationTime: compilationTime,
                    ExecutionTime: executionTime, ExitCode: runExitCode);
            if (errorOut.Contains("Killed") || errorOut.Contains("Terminated") || runExitCode == 124)
                return new CompileResponse(false, Error: "Time Limit Exceeded", Output: output,
                    CompilationTime: compilationTime, ExecutionTime: executionTime, ExitCode: runExitCode);

            return new CompileResponse(true, output, errorOut, CompilationTime: compilationTime,
                ExecutionTime: executionTime, ExitCode: runExitCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Compilation error");
            return new CompileResponse(false, Error: ex.Message, CompilationTime: compilationTime,
                ExecutionTime: executionTime);
        }
    }

    private async Task<JudgeResult> ExecuteJudgeAsync(string container, JudgeRequest request, CancellationToken ct)
    {
        var workspaceId = Guid.NewGuid().ToString("N");
        var sourceFile = $"code_{workspaceId}.c";
        var exeFile = $"code_{workspaceId}";

        var encodedCode = Convert.ToBase64String(Encoding.UTF8.GetBytes(request.Code));
        var gccFlags = $"-O{request.OptimizationLevel} -std=c11";

        // 1. Compile
        var compileCmd = new List<string>
        {
            "sh", "-c",
            $"echo \"{encodedCode}\" | base64 -d > {sourceFile} && gcc {sourceFile} -o {exeFile} {gccFlags} 2>&1"
        };

        var (compileStdout, compileStderr, compileExitCode) =
            await ExecuteSimpleCommandAsync(container, compileCmd, ct);
        if (compileExitCode != 0 || !string.IsNullOrWhiteSpace(compileStdout) ||
            !string.IsNullOrWhiteSpace(compileStderr))
            // Likely compile error if there is output from gcc (it's usually silent on success)
            // But warnings exist. Checking exit code is better but Docker.DotNet exec inspection involves another call.
            // TS version checked output including "error:".
            if ((compileStdout + compileStderr).Contains("error:"))
            {
                _ = CleanupFilesAsync(container, sourceFile);
                var error = (compileStdout + compileStderr).Trim();
                return CreateFailedJudgeResult(request, error);
            }

        var results = new List<TestCaseResult>();
        var passed = 0;
        var failed = 0;

        // 2. Run Test Cases
        foreach (var (testCase, index) in request.TestCases.Select((tc, i) => (tc, i + 1)))
        {
            var runCmdList = new List<string>();
            var runCmdString = $"timeout {Math.Ceiling(request.TimeLimit / 1000.0)}s ./{exeFile}";

            if (!string.IsNullOrEmpty(testCase.Input))
            {
                var encodedInput = Convert.ToBase64String(Encoding.UTF8.GetBytes(testCase.Input));
                runCmdList.Add("sh");
                runCmdList.Add("-c");
                runCmdList.Add($"echo \"{encodedInput}\" | base64 -d | {runCmdString}");
            }
            else
            {
                runCmdList.Add("sh");
                runCmdList.Add("-c");
                runCmdList.Add(runCmdString);
            }

            var sw = Stopwatch.StartNew();
            var (runStdout, runStderr, runExitCode) = await ExecuteSimpleCommandAsync(container, runCmdList, ct);
            sw.Stop();
            var executionTime = sw.ElapsedMilliseconds;

            // Normalize Output
            var actual = runStdout.Trim();
            var expected = testCase.ExpectedOutput.Trim();

            // Basic Error Detection
            string? error = null;
            if (runStderr.Contains("Segmentation fault") || actual.Contains("Segmentation fault"))
                error = "Segmentation fault";
            else if (runStderr.Contains("Killed") || runStderr.Contains("Terminated") || runExitCode == 124)
                error = "Time limit exceeded"; // timeout cmd exit code is 124

            var isPassed = error == null && actual == expected;
            if (isPassed) passed++;
            else failed++;

            results.Add(new TestCaseResult(index, isPassed, testCase.Input, expected, actual, error, executionTime));
        }

        _ = CleanupFilesAsync(container, sourceFile, exeFile);

        return new JudgeResult(passed, failed, request.TestCases.Count, results);
    }

    private JudgeResult CreateFailedJudgeResult(JudgeRequest request, string error)
    {
        var results = request.TestCases.Select((tc, i) =>
            new TestCaseResult(i + 1, false, tc.Input, tc.ExpectedOutput, null, error, null)).ToList();
        return new JudgeResult(0, request.TestCases.Count, request.TestCases.Count, results);
    }

    private async Task<(string Stdout, string Stderr, int ExitCode)> ExecuteSimpleCommandAsync(string container,
        IList<string> cmd, CancellationToken ct)
    {
        var execCreate = await _client.Exec.ExecCreateContainerAsync(container, new ContainerExecCreateParameters
        {
            Cmd = cmd,
            AttachStdout = true,
            AttachStderr = true,
            AttachStdin = false, // No longer attaching stdin
            User = "nobody"
        }, ct);


        using var stream = await _client.Exec.StartAndAttachContainerExecAsync(execCreate.ID, false, ct);

        // Read output
        var (stdout, stderr) = await ReadOutputAsync(stream, ct);

        // Inspect for Exit Code
        ContainerExecInspectResponse inspect = null;
        var remainingAttempts = 5;
        while (remainingAttempts-- > 0)
        {
            inspect = await _client.Exec.InspectContainerExecAsync(execCreate.ID, ct);
            if (!inspect.Running) break;
            await Task.Delay(50, ct);
        }

        return (stdout, stderr, (int)(inspect?.ExitCode ?? -1));
    }

    private async Task CleanupFilesAsync(string container, params string[] files)
    {
        try
        {
            var cmd = new List<string> { "rm", "-f" };
            cmd.AddRange(files);

            await _client.Exec.ExecCreateContainerAsync(container, new ContainerExecCreateParameters
            {
                Cmd = cmd,
                User = "root" // Cleanup as root
            });
            // Not waiting for execution to keep it fire-and-forget
        }
        catch
        {
            /* Ignore cleanup errors */
        }
    }

    private async Task<(string Stdout, string Stderr)> ReadOutputAsync(MultiplexedStream stream, CancellationToken ct)
    {
        // Docker.DotNet MultiplexedStream reading
        using var stdoutStream = new MemoryStream();
        using var stderrStream = new MemoryStream();

        await stream.CopyOutputToAsync(Stream.Null, stdoutStream, stderrStream, ct);

        return (Encoding.UTF8.GetString(stdoutStream.ToArray()), Encoding.UTF8.GetString(stderrStream.ToArray()));
    }
}
