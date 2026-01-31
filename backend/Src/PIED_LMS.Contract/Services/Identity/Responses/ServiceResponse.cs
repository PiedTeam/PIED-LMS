namespace PIED_LMS.Contract.Services.Identity.Responses;

public record ServiceResponse<T>(
    bool Success,
    string Message,
    T? Data = default,
    Dictionary<string, string[]>? Errors = null
);
