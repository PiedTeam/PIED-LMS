using System.Text.Json.Serialization;
using PIED_LMS.Contract.Services.Compiler.Responses;

namespace PIED_LMS.Application.UserCases.Compilers.Commands.Compile;

public record CompileCodeCommand(
    string Code,
    string Input = "",
    string Language = "c",
    int TimeLimit = 1000,
    int MemoryLimit = 128,
    int OptimizationLevel = 0
) : IRequest<CompileResponse>
{
    [JsonIgnore] public string Identifier { get; init; } = "anonymous";
}
