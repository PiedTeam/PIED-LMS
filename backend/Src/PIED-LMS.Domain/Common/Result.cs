namespace PIED_LMS.Domain.Common;

/// <summary>
///     Represents an error with a code and description for handling multi-language support on frontend
/// </summary>
public record Error
{
    public Error()
    {
    }

    public Error(string code, string description)
    {
        Code = code;
        Description = description;
    }

    /// <summary>
    ///     Error code for frontend to handle multi-language support and identify error type
    /// </summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>
    ///     Human-readable error description
    /// </summary>
    public string Description { get; init; } = string.Empty;

    public static Error Create(string code, string description) => new(code, description);
}

/// <summary>
///     Generic Result type for returning operation results with data and error information
/// </summary>
public record Result<T>
{
    private Result()
    {
    }

    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public Error? Error { get; init; }

    public static Result<T> Success(T data) => new() { IsSuccess = true, Data = data };

    public static Result<T> Failure(Error error) => new() { IsSuccess = false, Error = error };

    public static Result<T> Failure(string code, string description) =>
        new() { IsSuccess = false, Error = Error.Create(code, description) };

    /// <summary>
    ///     Implicit conversion from data to Result - allows: return data;
    /// </summary>
    public static implicit operator Result<T>(T data) => Success(data);

    /// <summary>
    ///     Implicit conversion from Error to Result - allows: return error;
    /// </summary>
    public static implicit operator Result<T>(Error error) => Failure(error);
}

/// <summary>
///     Non-generic Result type for operations that don't return data
/// </summary>
public record Result
{
    private Result()
    {
    }

    public bool IsSuccess { get; init; }
    public Error? Error { get; init; }

    public static Result Success() => new() { IsSuccess = true };

    public static Result Failure(Error error) => new() { IsSuccess = false, Error = error };

    public static Result Failure(string code, string description) =>
        new() { IsSuccess = false, Error = Error.Create(code, description) };

    /// <summary>
    ///     Implicit conversion from Error to Result - allows: return error;
    /// </summary>
    public static implicit operator Result(Error error) => Failure(error);
}
