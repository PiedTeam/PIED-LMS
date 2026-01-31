namespace PIED_LMS.Contract.Services.Compiler.Responses;

public record CompileResponse(
    bool Success,
    string Output = "",
    string Error = "",
    long ExecutionTime = 0,
    long CompilationTime = 0,
    int? ExitCode = null
);
