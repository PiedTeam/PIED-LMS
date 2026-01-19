namespace PIED_LMS.Infrastructure.Identity;

/// <summary>
///     Infrastructure UserProfile - ASP.NET Identity user for authentication and authorization
///     Separate from domain User entity to maintain clean architecture
/// </summary>
public sealed class UserProfile : IdentityUser<Guid>
{
    public UserProfile()
    {
        Id = Guid.CreateVersion7();
    }

    public UserProfile(string email, string userName) : this()
    {
        Email = email;
        UserName = userName ?? email;
    }

    /// <summary>
    ///     Reference to the domain User entity for business logic and profile data
    ///     This allows accessing the full user profile information
    /// </summary>
    public Guid? DomainUserId { get; init; }
}
