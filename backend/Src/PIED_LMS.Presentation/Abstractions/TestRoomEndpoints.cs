using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using PIED_LMS.Contract.Abstractions.Shared;
using PIED_LMS.Contract.Services.Identity;

namespace PIED_LMS.Presentation.Abstractions;

public class TestRoomEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/test-rooms")
            .WithName("TestRooms")
            .WithOpenApi()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Teacher" }); 
        group.MapPost("/", CreateTestRoom)
            .WithName("CreateTestRoom")
            .WithOpenApi()
            .Produces<Result<Guid>>();
    }
    private static async Task<IResult> CreateTestRoom(
        CreateRoomsCommand request,
        IMediator mediator)
    {
        var result = await mediator.Send(request);
        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
    }
}
