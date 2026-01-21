namespace PIED_LMS.Contract.Abstractions.Shared;

public interface IValidationResult
{
    public static readonly Error ValidationError = new("Validation Error", "A validation error occurred");
    Error[] Errors { get; }
}
