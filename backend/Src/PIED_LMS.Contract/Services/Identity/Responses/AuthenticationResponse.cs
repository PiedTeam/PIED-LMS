namespace PIED_LMS.Contract.Services.Identity.Responses;

public record AuthenticationResponse(
    string AccessToken,
    string RefreshToken,
    string Email,
    string FirstName,
    string LastName
);
