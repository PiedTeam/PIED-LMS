using PIED_LMS.Presentation.Extensions;

namespace PIED_LMS.Presentation.APIs.Compiler;

public class CompilerModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/compiler").WithTags("Compiler");

        group.MapPost("/compile", async (CompileRequest request, ISender sender, HttpContext context) =>
        {
            var identifier = context.GetIdentifier();

            // Map Request DTO to Command
            var command = new CompileCodeCommand(
                request.Code,
                request.Input,
                request.Language,
                request.TimeLimit,
                request.MemoryLimit,
                request.OptimizationLevel
            )
            { Identifier = identifier };

            var result = await sender.Send(command);
            return Results.Ok(result);
        });

        group.MapPost("/judge", async (JudgeRequest request, ISender sender, HttpContext context) =>
        {
            var identifier = context.GetIdentifier();

            // Map Request DTO to Command
            var command = new JudgeSubmissionCommand(
                request.Code,
                request.TestCases,
                request.TimeLimit,
                request.MemoryLimit,
                request.OptimizationLevel
            )
            { Identifier = identifier };

            var result = await sender.Send(command);
            return Results.Ok(result);
        });

        group.MapPost("/judge-from-file", async (JudgeFromFileRequest request, ISender sender, HttpContext context) =>
        {
            var identifier = context.GetIdentifier();

            // Map Request DTO to Command
            var command = new JudgeFromFileCommand(
                request.Code,
                request.RoomId,
                request.QuestionId,
                request.IncludePrivate,
                request.TimeLimit,
                request.MemoryLimit,
                request.OptimizationLevel
            )
            { Identifier = identifier };

            var result = await sender.Send(command);
            return Results.Ok(result);
        });
    }
}
