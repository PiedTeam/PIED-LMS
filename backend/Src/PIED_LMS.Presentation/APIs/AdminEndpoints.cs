using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PIED_LMS.Contract.Services.Identity;
using Carter;
using Microsoft.AspNetCore.Authorization;

namespace PIED_LMS.Presentation.APIs;

public class AdminEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin")
            .WithName("Admin")
            .WithOpenApi()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Administrator" }) // Uncomment when ready
            ;

        group.MapPost("/students/import", ImportStudents)
            .WithName("ImportStudents")
            .WithOpenApi()
            .Produces<ServiceResponse<string>>();
            
        group.MapPost("/mentors/{userId}/approve", ApproveMentor)
            .WithName("ApproveMentor")
            .WithOpenApi()
            .Produces<ServiceResponse<string>>();
    }

    private static async Task<IResult> ImportStudents(
        ImportStudentsCommand request,
        IMediator mediator)
    {
        var result = await mediator.Send(request);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static async Task<IResult> ApproveMentor(
        Guid userId,
        IMediator mediator)
    {
        var command = new ApproveMentorCommand(userId);
        var result = await mediator.Send(command);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}
