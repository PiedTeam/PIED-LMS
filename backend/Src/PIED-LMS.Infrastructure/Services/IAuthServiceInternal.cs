using PIED_LMS.Application.DTOs.Auth;
using PIED_LMS.Domain.Common;
using PIED_LMS.Infrastructure.Auth;

namespace PIED_LMS.Infrastructure.Services;

/// <summary>
///     Internal auth service interface that returns refresh tokens
///     This is only visible within the Infrastructure layer
/// </summary>
internal interface IAuthServiceInternal
{
    Task<Result<AuthResult>> RegisterAsyncInternal(
        RegisterRequest request, CancellationToken cancellationToken = default);

    Task<Result<AuthResult>> LoginAsyncInternal(
        LoginRequest request, CancellationToken cancellationToken = default);

    Task<Result<AuthResult>> RefreshTokenAsyncInternal(
        string accessToken, string refreshToken, CancellationToken cancellationToken = default);
}
