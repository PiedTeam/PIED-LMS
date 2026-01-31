namespace PIED_LMS.Contract.Services.Compiler.Requests;

public record JudgeRequest(
    string Code,
    IList<TestCase> TestCases,
    int TimeLimit = 1000,
    int MemoryLimit = 128,
    int OptimizationLevel = 0
);
