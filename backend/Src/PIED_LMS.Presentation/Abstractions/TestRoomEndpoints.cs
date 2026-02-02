using Microsoft.AspNetCore.Authorization;
using PIED_LMS.Contract.Services.Identity;
using PIED_LMS.Domain.Constants;

namespace PIED_LMS.Presentation.Abstractions;

public class TestRoomEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/test-rooms")
            .WithName("TestRooms")
            .WithOpenApi()
            .RequireAuthorization(new AuthorizeAttribute { Roles = RoleConstants.Teacher }); 
        group.MapPost("/", CreateTestRoom)
            .WithName("CreateTestRoom")
            .WithOpenApi()
            .Produces<ServiceResponse<Guid>>()
            .Produces<ServiceResponse<Guid>>(StatusCodes.Status400BadRequest);
    }
    private static async Task<IResult> CreateTestRoom(
        CreateRoomsCommand request,
        IMediator mediator)
    {
        var result = await mediator.Send(request);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}
