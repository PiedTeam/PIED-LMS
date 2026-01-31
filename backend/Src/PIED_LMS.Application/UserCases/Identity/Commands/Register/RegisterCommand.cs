using PIED_LMS.Contract.Services.Identity.Responses;

namespace PIED_LMS.Application.UserCases.Identity.Commands.Register;

public record RegisterCommand(
    string Email,
    string FirstName,
    string LastName,
    string Password,
    string ConfirmPassword
) : IRequest<ServiceResponse<RegisterResponse>>;
