namespace PIED_LMS.Contract.Services.Identity.Responses;

public record UserResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    bool IsActive,
    DateTime CreatedAt,
    List<string> Roles
);
