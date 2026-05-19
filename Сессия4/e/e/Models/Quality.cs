using System.ComponentModel.DataAnnotations;

namespace AgroControlAPI.Models
{
    public class QualityControl
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BatchId { get; set; }

        [Required]
        [MaxLength(20)]
        public string SampleType { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ParameterName { get; set; } = string.Empty;

        public decimal? MeasuredValue { get; set; }

        [MaxLength(50)]
        public string? StandardValue { get; set; }

        [MaxLength(10)]
        public string? Unit { get; set; }

        [MaxLength(20)]
        public string? Result { get; set; }

        [MaxLength(20)]
        public string? Decision { get; set; }

        public int? AnalystId { get; set; }

        public DateTime AnalysisDate { get; set; } = DateTime.UtcNow;

        public string? Comment { get; set; }

        // Навигационные свойства
        public virtual Batch? Batch { get; set; }
    }

    public class Deviation
    {
        [Key]
        public int Id { get; set; }

        public int? BatchId { get; set; }

        public int? StepExecutionId { get; set; }

        [MaxLength(50)]
        public string? ParameterName { get; set; }

        public string? PlannedValue { get; set; }

        public string? ActualValue { get; set; }

        [MaxLength(20)]
        public string Severity { get; set; } = "warning";

        public string? Description { get; set; }

        public int? ReportedBy { get; set; }

        public DateTime ReportedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ResolvedAt { get; set; }

        public string? ResolutionComment { get; set; }

        // Навигационные свойства
        public virtual Batch? Batch { get; set; }
        public virtual BatchStepExecution? StepExecution { get; set; }
    }

    public class Notification
    {
        [Key]
        public int Id { get; set; }

        public int? UserId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Type { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Навигационные свойства
        public virtual User? User { get; set; }
    }
}