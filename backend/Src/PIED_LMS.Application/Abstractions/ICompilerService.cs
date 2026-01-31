namespace PIED_LMS.Application.Abstractions;

public interface ICompilerService
{
    Task<CompileResponse> CompileAsync(CompileRequest request, string identifier, CancellationToken cancellationToken = default);
    Task<JudgeResult> JudgeAsync(JudgeRequest request, string identifier, CancellationToken cancellationToken = default);
}
