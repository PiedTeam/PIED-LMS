namespace PIED_LMS.Application.UserCases.Compilers.Commands.Compile;

public class CompileCodeHandler(ICompilerService compilerService) : IRequestHandler<CompileCodeCommand, CompileResponse>
{
    public async Task<CompileResponse> Handle(CompileCodeCommand request, CancellationToken cancellationToken)
    {
        var compileRequest = new CompileRequest(
            request.Code,
            request.Input,
            request.Language,
            request.TimeLimit,
            request.MemoryLimit,
            request.OptimizationLevel
        );

        return await compilerService.CompileAsync(compileRequest, request.Identifier, cancellationToken);
    }
}
