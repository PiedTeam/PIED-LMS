using PIED_LMS.Presentation.Extensions;

namespace PIED_LMS.Presentation.APIs.Compiler;

public class CompilerModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/compiler").WithTags("Compiler");

        group.MapPost("/compile", async (CompileRequest request, IMapper mapper, ISender sender, HttpContext context) =>
        {
            var identifier = context.GetIdentifier();
            var command = mapper.Map<CompileCodeCommand>(request) with { Identifier = identifier };

            var result = await sender.Send(command);
            return Results.Ok(result);
        });

        group.MapPost("/judge", async (JudgeRequest request, IMapper mapper, ISender sender, HttpContext context) =>
        {
            var identifier = context.GetIdentifier();
            var command = mapper.Map<JudgeSubmissionCommand>(request) with { Identifier = identifier };

            var result = await sender.Send(command);
            return Results.Ok(result);
        });

        group.MapPost("/judge-from-file", async (JudgeFromFileRequest request, IMapper mapper, ISender sender, HttpContext context) =>
        {
            var identifier = context.GetIdentifier();
            var command = mapper.Map<JudgeFromFileCommand>(request) with { Identifier = identifier };

            var result = await sender.Send(command);
            return Results.Ok(result);
        });
    }
}
