using System;
using System.Collections.Generic;
using PIED_LMS.Contract.Abstractions.Shared;

namespace PIED_LMS.Contract.Services.Identity;

// Register Commands
public record RegisterCommand(
    string Email,
    string FirstName,
    string LastName,
    string Password,
    string ConfirmPassword
) : IRequest<ServiceResponse<RegisterResponse>>;

// Login Commands
public record LoginCommand(
    string Email,
    string Password
) : IRequest<ServiceResponse<LoginResult>>;

// Login Result (internal use - contains response and refresh token)
public record LoginResult(LoginResponse Response, string RefreshToken);

// Change Password Commands
public record ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword
) : IRequest<ServiceResponse<string>>;

// Assign Role Commands
public record AssignRoleCommand(
    Guid UserId,
    string RoleName
) : IRequest<ServiceResponse<string>>;

// Logout Commands
public record LogoutCommand(
    Guid UserId,
    string RefreshToken
) : IRequest<ServiceResponse<string>>;

// Refresh Token Commands
public record RefreshTokenCommand(
    string RefreshToken
) : IRequest<ServiceResponse<RefreshTokenResponse>>;

public record CreateRoomsCommand(
     string Name,
    string? Description,
    DateTime StartTime,
    DateTime EndTime
    ) : IRequest<Result<Guid>>;

// Import Student Command
public record StudentImportDto(string Email, string FirstName, string LastName);
public record ImportStudentsCommand(List<StudentImportDto> Students) : IRequest<ServiceResponse<string>>;

// Mentor Registration & Approval Commands
public record RegisterMentorCommand(string Email, string FirstName, string LastName, string Bio, string Password) : IRequest<ServiceResponse<string>>;
public record ApproveMentorCommand(Guid UserId) : IRequest<ServiceResponse<string>>;
