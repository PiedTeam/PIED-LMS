namespace PIED_LMS.Contract.Services.Identity.Requests;

public record AssignRoleRequest(
    Guid UserId,
    string RoleName
);
