namespace PIED_LMS.Contract.Services.Identity.Requests;

public record LoginRequest(
    string Email,
    string Password
);
