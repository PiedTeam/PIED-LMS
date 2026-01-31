using System.Text.Json.Serialization;

namespace PIED_LMS.Application.UserCases.Compilers.Commands.JudgeFromFile;

public record JudgeFromFileCommand(
    string Code,
    string RoomId,
    string QuestionId,
    bool IncludePrivate = false,
    int TimeLimit = 1000,
    int MemoryLimit = 128,
    int OptimizationLevel = 0
) : IRequest<JudgeResult>
{
    [JsonIgnore] public string Identifier { get; init; } = "anonymous";
}
