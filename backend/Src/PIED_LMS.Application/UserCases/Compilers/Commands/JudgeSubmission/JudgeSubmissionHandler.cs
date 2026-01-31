namespace PIED_LMS.Application.UserCases.Compilers.Commands.JudgeSubmission;

public class JudgeSubmissionHandler(ICompilerService compilerService)
    : IRequestHandler<JudgeSubmissionCommand, JudgeResult>
{
    public async Task<JudgeResult> Handle(JudgeSubmissionCommand request, CancellationToken cancellationToken)
    {
        var judgeRequest = new JudgeRequest(
            request.Code,
            request.TestCases,
            request.TimeLimit,
            request.MemoryLimit,
            request.OptimizationLevel
        );

        return await compilerService.JudgeAsync(judgeRequest, request.Identifier, cancellationToken);
    }
}
