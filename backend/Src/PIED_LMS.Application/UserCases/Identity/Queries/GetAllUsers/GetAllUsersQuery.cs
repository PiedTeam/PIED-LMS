namespace PIED_LMS.Application.UserCases.Identity.Queries.GetAllUsers;

public record GetAllUsersQuery(int PageNumber = 1, int PageSize = 10)
    : IRequest<ServiceResponse<PaginatedResponse<UserResponse>>>;
