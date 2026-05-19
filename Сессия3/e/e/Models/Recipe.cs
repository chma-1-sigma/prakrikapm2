using System.ComponentModel.DataAnnotations;

namespace AgroControlAPI.Models
{
    public class Recipe
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        public int Version { get; set; } = 1;

        [MaxLength(20)]
        public string Status { get; set; } = "draft";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public string? Description { get; set; }

        // Навигационные свойства
        public virtual Product? Product { get; set; }
        public virtual ICollection<RecipeComponent> Components { get; set; } = new List<RecipeComponent>();
    }

    public class RecipeComponent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RecipeId { get; set; }

        [Required]
        public int RawMaterialId { get; set; }

        [Range(0.01, 100)]
        public decimal QuantityPercent { get; set; }

        public int LoadOrder { get; set; }

        public decimal ToleranceMin { get; set; } = 0;

        public decimal ToleranceMax { get; set; } = 0;

        public bool IsCritical { get; set; } = false;

        // Навигационные свойства
        public virtual Recipe? Recipe { get; set; }
        public virtual RawMaterial? RawMaterial { get; set; }
    }

    public class RawMaterial
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Category { get; set; }

        [MaxLength(10)]
        public string? Unit { get; set; }

        [MaxLength(100)]
        public string? Supplier { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}