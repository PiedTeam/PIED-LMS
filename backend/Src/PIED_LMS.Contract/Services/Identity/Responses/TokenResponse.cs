namespace PIED_LMS.Contract.Services.Identity.Responses;

public record TokenResponse(
    string AccessToken,
    string RefreshToken
);
