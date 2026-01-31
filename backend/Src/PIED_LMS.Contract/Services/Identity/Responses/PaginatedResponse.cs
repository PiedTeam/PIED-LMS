namespace PIED_LMS.Contract.Services.Identity.Responses;

public record PaginatedResponse<T>(
    List<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize
);
