namespace PIED_LMS.Domain.Entities;

public class User
{
    public User(string email, string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email không được để trống.");

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

    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public string Email { get; private set; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string FullName => $"{FirstName} {LastName}".Trim();
}
