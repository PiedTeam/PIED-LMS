namespace PIED_LMS.Domain.Compiler;

public enum CompilerErrorCode
{
    Unknown = 0,
    CompilationFailed = 1,
    TimeLimitExceeded = 2,
    MemoryLimitExceeded = 3,
    RuntimeError = 4,
    RateLimitExceeded = 429,
    InvalidCode = 400
}
