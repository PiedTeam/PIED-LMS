namespace PIED_LMS.Contract.Services.Identity.Responses;

public record RegisterResponse(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName
);
