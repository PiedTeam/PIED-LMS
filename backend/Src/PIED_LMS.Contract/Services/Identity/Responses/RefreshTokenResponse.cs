namespace PIED_LMS.Contract.Services.Identity.Responses;

public record RefreshTokenResponse(
    string AccessToken,
    [property: JsonIgnore] string RefreshToken
);
