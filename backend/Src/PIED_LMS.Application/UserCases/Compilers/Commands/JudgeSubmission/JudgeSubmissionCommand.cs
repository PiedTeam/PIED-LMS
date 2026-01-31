using System.Text.Json.Serialization;

namespace PIED_LMS.Application.UserCases.Compilers.Commands.JudgeSubmission;

public record JudgeSubmissionCommand(
    string Code,
    IList<TestCase> TestCases,
    int TimeLimit = 1000,
    int MemoryLimit = 128,
    int OptimizationLevel = 0
) : IRequest<JudgeResult>
{
    [JsonIgnore] public string Identifier { get; init; } = "anonymous";
}
