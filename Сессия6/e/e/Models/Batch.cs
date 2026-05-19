using System.ComponentModel.DataAnnotations;

namespace AgroControlAPI.Models
{
    public class ProductionOrder
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string OrderNumber { get; set; } = string.Empty;

        [Required]
        public int RecipeId { get; set; }

        public int PlannedQuantityKg { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "planned";

        public int Priority { get; set; } = 1;

        public DateTime? PlannedStartDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Навигационные свойства
        public virtual Recipe? Recipe { get; set; }
        public virtual ICollection<Batch> Batches { get; set; } = new List<Batch>();
    }

    public class Batch
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string BatchNumber { get; set; } = string.Empty;

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        public int? RecipeVersion { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "planned";

        public int ActualQuantityKg { get; set; } = 0;

        [MaxLength(20)]
        public string LabStatus { get; set; } = "pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Навигационные свойства
        public virtual ProductionOrder? Order { get; set; }
        public virtual Product? Product { get; set; }
        public virtual ICollection<BatchStepExecution> StepExecutions { get; set; } = new List<BatchStepExecution>();
        public virtual ICollection<QualityControl> QualityControls { get; set; } = new List<QualityControl>();
    }

    public class BatchStepExecution
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BatchId { get; set; }

        [Required]
        public int StepId { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "pending";

        public int? OperatorId { get; set; }

        public bool DeviationFlag { get; set; } = false;

        public string? OperatorComment { get; set; }

        // Навигационные свойства
        public virtual Batch? Batch { get; set; }
        public virtual TechCardStep? Step { get; set; }
        public virtual ICollection<StepActualParameter> ActualParameters { get; set; } = new List<StepActualParameter>();
    }

    public class StepActualParameter
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StepExecutionId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ParameterName { get; set; } = string.Empty;

        public decimal? ActualValue { get; set; }

        [MaxLength(10)]
        public string? Unit { get; set; }

        public bool IsDeviated { get; set; } = false;

        public string? DeviationReason { get; set; }

        // Навигационные свойства
        public virtual BatchStepExecution? StepExecution { get; set; }
    }
}