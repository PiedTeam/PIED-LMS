namespace PIED_LMS.Contract.Services.Compiler.Responses;

public record JudgeResult(
    int Passed,
    int Failed,
    int Total,
    IList<TestCaseResult> Results
);
