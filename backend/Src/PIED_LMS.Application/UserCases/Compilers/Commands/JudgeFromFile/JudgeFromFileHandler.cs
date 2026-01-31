namespace PIED_LMS.Application.UserCases.Compilers.Commands.JudgeFromFile;

public class JudgeFromFileHandler(ICompilerService compilerService) : IRequestHandler<JudgeFromFileCommand, JudgeResult>
{
    public async Task<JudgeResult> Handle(JudgeFromFileCommand request, CancellationToken cancellationToken)
    {
        // NOT IMPLEMENTED: Mock
        await Task.Yield();
        return new JudgeResult(0, 0, 0, new List<TestCaseResult>
        {
            new(0, false, "", "", null, "JudgeFromFile not fully implemented (missing TestCaseService)", null)
        });
    }
}
