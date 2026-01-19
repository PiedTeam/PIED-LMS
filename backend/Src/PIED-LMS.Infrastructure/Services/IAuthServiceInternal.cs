using PIED_LMS.Application.DTOs.Auth;
using PIED_LMS.Domain.Common;

namespace PIED_LMS.Infrastructure.Services;

/// <summary>
///     Internal auth service interface that returns refresh tokens
///     This is only visible within the Infrastructure layer
/// </summary>
internal interface IAuthServiceInternal
{
  Task<Result<(AuthResponse PublicResponse, string RefreshToken)>> RegisterAsyncInternal(
      RegisterRequest request, CancellationToken cancellationToken = default);

  Task<Result<(AuthResponse PublicResponse, string RefreshToken)>> LoginAsyncInternal(
      LoginRequest request, CancellationToken cancellationToken = default);

  Task<Result<(AuthResponse PublicResponse, string RefreshToken)>> RefreshTokenAsyncInternal(
      string accessToken, string refreshToken, CancellationToken cancellationToken = default);
}
