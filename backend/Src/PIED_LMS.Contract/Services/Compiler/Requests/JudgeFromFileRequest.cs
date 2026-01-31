namespace PIED_LMS.Contract.Services.Compiler.Requests;

public record JudgeFromFileRequest(
    string Code,
    string RoomId,
    string QuestionId,
    bool IncludePrivate = false,
    int TimeLimit = 1000,
    int MemoryLimit = 128,
    int OptimizationLevel = 0
);
