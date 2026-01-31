using PIED_LMS.Contract.Services.Identity.Responses;

namespace PIED_LMS.Application.UserCases.Identity.Commands.AssignRole;

public record AssignRoleCommand(
    Guid UserId,
    string RoleName
) : IRequest<ServiceResponse<string>>;
