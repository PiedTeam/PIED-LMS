
using Microsoft.AspNetCore.Http;

using PIED_LMS.Contract.Services.Identity;
using PIED_LMS.Domain.Abstractions;
using PIED_LMS.Domain.Entities;

namespace PIED_LMS.Application.UserCases.Commands.Room;

public class CreateTestRoomHandler(
    IUnitOfWork unitOfWork,

    IHttpContextAccessor httpContextAccessor
) : IRequestHandler<CreateRoomsCommand, ServiceResponse<Guid>>
{
    public async Task<ServiceResponse<Guid>> Handle(CreateRoomsCommand request, CancellationToken ct)
    {
        if (request.StartTime >= request.EndTime)
            return new ServiceResponse<Guid>(false, "Start time must be before end time.");
        var userIdString = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var teacherId))
            return new ServiceResponse<Guid>(false, "User not found.");
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
        await unitOfWork.Repository<TestRoom>().AddAsync(room, ct);
        await unitOfWork.CommitAsync(ct);
        // Return Success
        return new ServiceResponse<Guid>(true, "Room created successfully", room.Id);
    }
    private static string GenerateJoinCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return System.Security.Cryptography.RandomNumberGenerator.GetString(chars, 6);
    }
}

