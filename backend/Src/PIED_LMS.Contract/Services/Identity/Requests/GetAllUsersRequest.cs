namespace PIED_LMS.Contract.Services.Identity.Requests;

public record GetAllUsersRequest(
    int PageNumber = 1,
    int PageSize = 10
);
