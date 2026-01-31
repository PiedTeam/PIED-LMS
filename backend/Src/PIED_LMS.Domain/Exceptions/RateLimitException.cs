namespace PIED_LMS.Domain.Exceptions;

public class RateLimitException(string message, TimeSpan? retryAfter = null)
    : DomainException("Rate Limit Exceeded", message)
{
    public TimeSpan? RetryAfter { get; } = retryAfter;
}
