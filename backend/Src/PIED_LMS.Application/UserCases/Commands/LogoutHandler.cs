using PIED_LMS.Contract.Services.Identity;

namespace PIED_LMS.Application.UserCases.Commands;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, ServiceResponse<string>>
{
    public async Task<ServiceResponse<string>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        try
        {
            return new ServiceResponse<string>(true, "Logout successful", "User logged out");
        }
        catch (Exception ex)
        {
            return new ServiceResponse<string>(false, $"Logout failed: {ex.Message}");
        }
    }
}
