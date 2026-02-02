using System.ComponentModel.DataAnnotations;

namespace PIED_LMS.Application.Options;

public class ImportSettings
{
    public const string SectionName = "ImportSettings";

    [Range(1, 10000, ErrorMessage = "Maximum batch size must be between 1 and 10,000")]
    public int MaxBatchSize { get; set; } = 1000;

    [Range(1, 1000, ErrorMessage = "Background processing threshold must be between 1 and 1,000")]
    public int BackgroundProcessingThreshold { get; set; } = 100;

    [Range(1, 3600, ErrorMessage = "Timeout must be between 1 and 3600 seconds")]
    public int TimeoutSeconds { get; set; } = 300; // 5 minutes
}
