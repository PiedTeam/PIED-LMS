namespace PIED_LMS.Application.Options;

public class JwtOption
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; }
    public int RefreshTokenExpirationDays { get; set; } = 7;
}
