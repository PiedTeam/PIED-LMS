namespace PIED_LMS.Contract.Services.Identity.Responses;

public record LoginResponse(
    string AccessToken,
    string Email,
    string FirstName,
    string LastName
);
