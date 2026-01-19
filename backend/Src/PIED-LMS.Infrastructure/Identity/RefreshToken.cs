namespace PIED_LMS.Infrastructure.Identity;

public sealed class RefreshToken
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public required string TokenHash { get; init; }
    public required Guid UserId { get; init; }
    public required DateTime ExpiresAt { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? RevokedAt { get; set; }
    public bool IsActive => RevokedAt == null && ExpiresAt > DateTime.UtcNow;

    public UserProfile User { get; init; } = null!;
}
