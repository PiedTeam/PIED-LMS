using System.Text.Json.Serialization;
using PIED_LMS.Contract.Services.Compiler.Requests;
using PIED_LMS.Contract.Services.Compiler.Responses;

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
