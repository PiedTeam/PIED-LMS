using PIED_LMS.Contract.Services.Identity.Responses;

namespace PIED_LMS.Application.UserCases.Identity.Queries.GetUserById;

public record GetUserByIdQuery(Guid UserId) : IRequest<ServiceResponse<UserResponse>>;
