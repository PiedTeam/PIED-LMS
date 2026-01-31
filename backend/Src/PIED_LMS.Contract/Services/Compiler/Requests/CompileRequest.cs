namespace PIED_LMS.Contract.Services.Compiler.Requests;

public record CompileRequest(
    string Code,
    string Input = "",
    string Language = "c",
    int TimeLimit = 1000,
    int MemoryLimit = 128,
    int OptimizationLevel = 0
);
