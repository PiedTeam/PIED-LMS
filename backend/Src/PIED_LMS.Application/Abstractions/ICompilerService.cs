using PIED_LMS.Contract.Services.Compiler.Requests;
using PIED_LMS.Contract.Services.Compiler.Responses;

namespace PIED_LMS.Application.Abstractions;

public interface ICompilerService
{
    Task<CompileResponse> CompileAsync(CompileRequest request, string identifier, CancellationToken cancellationToken = default);
    Task<JudgeResult> JudgeAsync(JudgeRequest request, string identifier, CancellationToken cancellationToken = default);
}
