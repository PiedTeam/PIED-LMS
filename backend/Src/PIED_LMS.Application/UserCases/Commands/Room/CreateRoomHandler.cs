using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using PIED_LMS.Contract.Abstractions.Shared;
using PIED_LMS.Contract.Services.Identity;
using PIED_LMS.Domain.Abstractions;
using PIED_LMS.Domain.Entities;

namespace PIED_LMS.Application.UserCases.Commands.Room;

public class CreateTestRoomHandler(
    IUnitOfWork unitOfWork,
    // Add UserContext service here to get Current Teacher ID
    IHttpContextAccessor httpContextAccessor
) : IRequestHandler<CreateRoomsCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateRoomsCommand request, CancellationToken ct)
    {
        if (request.StartTime >= request.EndTime)
            return Result.Failure<Guid>(new Error("TestRoom.InvalidTime", "Start time must be before end time."));
        var userIdString = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var teacherId))
            return Result.Failure<Guid>(new Error("Auth.Unauthorized", "User not found."));
        var room = new TestRoom
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            JoinCode = GenerateJoinCode(), 
            CreatedBy = teacherId,
            CreatedAt = DateTime.UtcNow
        };
        // 4. Save to DB
        await unitOfWork.Repository<TestRoom>().AddAsync(room);
        await unitOfWork.CommitAsync(ct);
        // Return Success
        return Result.Success(room.Id);
    }
    private static string GenerateJoinCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[new Random().Next(s.Length)]).ToArray());
    }
}

