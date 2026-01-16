using System.Net.Mail;

namespace PIED_LMS.Domain.Entities;

public class User
{
    /// <summary>
    /// Parameterless constructor for EF Core materialization.
    /// </summary>
    private User()
    {
        Email = null!;
        FirstName = null!;
        LastName = null!;
    }

    public User(string email, string firstName, string lastName)
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

        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }

    private static void ValidateEmailFormat(string email)
    {
        if (!MailAddress.TryCreate(email, out _))
            throw new ArgumentException("Email không hợp lệ.");
    }

    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string FullName => $"{FirstName} {LastName}".Trim();
}
