using System.Net.Mail;

using PIED_LMS.Domain.Common;

namespace PIED_LMS.Domain.Entities;

/// <summary>
///     Domain User entity - represents user profile information and business logic
///     Independent of infrastructure concerns like authentication and identity management
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    ///     Parameterless constructor for EF Core materialization
    /// </summary>
    private User()
    {
    }

    public User(Guid id, string email, string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email không được để trống.");
        ValidateEmailFormat(email);

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name là bắt buộc.");
        if (firstName.Length > 100)
            throw new ArgumentException("First name không được quá 100 ký tự.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name là bắt buộc.");
        if (lastName.Length > 100)
            throw new ArgumentException("Last name không được quá 100 ký tự.");

        Id = id;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }

    public string Email { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    public void UpdateProfile(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name là bắt buộc.");
        if (firstName.Length > 100)
            throw new ArgumentException("First name không được quá 100 ký tự.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name là bắt buộc.");
        if (lastName.Length > 100)
            throw new ArgumentException("Last name không được quá 100 ký tự.");

        FirstName = firstName;
        LastName = lastName;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateEmailFormat(string email)
    {
        if (!MailAddress.TryCreate(email, out _))
            throw new ArgumentException("Email không hợp lệ.");
    }
}
