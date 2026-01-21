namespace PIED_LMS.Application.Abstractions;

public interface IJwtTokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    (ClaimsPrincipal, bool) GetPrincipalFromExpiredToken(string token);
}
