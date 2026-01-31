namespace PIED_LMS.Contract.Services.Compiler.Responses;

public record TestCaseResult(
    int TestCaseNumber,
    bool Passed,
    string Input,
    string ExpectedOutput,
    string? ActualOutput,
    string? Error,
    long? ExecutionTime
);
