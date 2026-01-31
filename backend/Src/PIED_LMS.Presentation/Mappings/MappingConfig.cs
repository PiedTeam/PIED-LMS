using Mapster;

namespace PIED_LMS.Presentation.Mappings;

public static class MappingConfig
{
    public static TypeAdapterConfig GetTypeAdapterConfig()
    {
        var config = new TypeAdapterConfig();

        // Identity Mappings
        ConfigureIdentityMappings(config);

        // Compiler Mappings
        ConfigureCompilerMappings(config);

        return config;
    }

    private static void ConfigureIdentityMappings(TypeAdapterConfig config)
    {
        // RegisterRequest -> RegisterCommand
        config.NewConfig<RegisterRequest, RegisterCommand>();

        // LoginRequest -> LoginCommand
        config.NewConfig<LoginRequest, LoginCommand>();

        // ChangePasswordRequest -> ChangePasswordCommand (needs UserId from context)
        // This will be mapped manually since it requires additional context

        // AssignRoleRequest -> AssignRoleCommand
        config.NewConfig<AssignRoleRequest, AssignRoleCommand>();

        // GetAllUsersRequest -> GetAllUsersQuery
        config.NewConfig<GetAllUsersRequest, GetAllUsersQuery>();
    }

    private static void ConfigureCompilerMappings(TypeAdapterConfig config)
    {
        // CompileRequest -> CompileCodeCommand
        config.NewConfig<CompileRequest, CompileCodeCommand>()
            .Map(dest => dest.Code, src => src.Code)
            .Map(dest => dest.Input, src => src.Input)
            .Map(dest => dest.Language, src => src.Language)
            .Map(dest => dest.TimeLimit, src => src.TimeLimit)
            .Map(dest => dest.MemoryLimit, src => src.MemoryLimit)
            .Map(dest => dest.OptimizationLevel, src => src.OptimizationLevel);

        // JudgeRequest -> JudgeSubmissionCommand
        config.NewConfig<JudgeRequest, JudgeSubmissionCommand>()
            .Map(dest => dest.Code, src => src.Code)
            .Map(dest => dest.TestCases, src => src.TestCases)
            .Map(dest => dest.TimeLimit, src => src.TimeLimit)
            .Map(dest => dest.MemoryLimit, src => src.MemoryLimit)
            .Map(dest => dest.OptimizationLevel, src => src.OptimizationLevel);

        // JudgeFromFileRequest -> JudgeFromFileCommand
        config.NewConfig<JudgeFromFileRequest, JudgeFromFileCommand>()
            .Map(dest => dest.Code, src => src.Code)
            .Map(dest => dest.RoomId, src => src.RoomId)
            .Map(dest => dest.QuestionId, src => src.QuestionId)
            .Map(dest => dest.IncludePrivate, src => src.IncludePrivate)
            .Map(dest => dest.TimeLimit, src => src.TimeLimit)
            .Map(dest => dest.MemoryLimit, src => src.MemoryLimit)
            .Map(dest => dest.OptimizationLevel, src => src.OptimizationLevel);
    }
}
