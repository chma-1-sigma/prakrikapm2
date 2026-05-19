using System.ComponentModel.DataAnnotations;

namespace AgroControlAPI.Models
{
    public class TechCard
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        public int Version { get; set; } = 1;

        [MaxLength(20)]
        public string Status { get; set; } = "draft";

        public bool IsActive { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public string? Description { get; set; }

        // Навигационные свойства
        public virtual Product? Product { get; set; }
        public virtual ICollection<TechCardStep> Steps { get; set; } = new List<TechCardStep>();
    }

    public class TechCardStep
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TechCardId { get; set; }

        public int StepOrder { get; set; }

        [Required]
        [MaxLength(100)]
        public string StepName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? StepType { get; set; }

        public int? EquipmentId { get; set; }

        public int? PlannedDurationMin { get; set; }

        public bool IsMandatory { get; set; } = true;

        public string? Instruction { get; set; }

        // Навигационные свойства
        public virtual TechCard? TechCard { get; set; }
        public virtual ICollection<StepParameter> Parameters { get; set; } = new List<StepParameter>();
    }

    public class StepParameter
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StepId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ParameterName { get; set; } = string.Empty;

        public decimal? PlannedValue { get; set; }

        [MaxLength(10)]
        public string? Unit { get; set; }

        public decimal? ToleranceMin { get; set; }

        public decimal? ToleranceMax { get; set; }

        public bool IsCritical { get; set; } = false;

        // Навигационные свойства
        public virtual TechCardStep? Step { get; set; }
    }
}