using PIED_LMS.Contract.Services.Identity;

namespace PIED_LMS.Presentation.APIs;

public class MentorEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/mentors")
            .WithName("Mentors")
            .WithOpenApi();
        group.MapPost("/request", RegisterMentor)
            .WithName("RegisterMentor")
            .WithOpenApi()
            .Produces<ServiceResponse<string>>()
            .Produces<ServiceResponse<string>>(StatusCodes.Status400BadRequest);
    }
    private static async Task<IResult> RegisterMentor(
        RegisterMentorCommand request,
        IMediator mediator)
    {
        var result = await mediator.Send(request);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}
