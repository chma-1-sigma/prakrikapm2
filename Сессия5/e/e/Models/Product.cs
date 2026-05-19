using System.ComponentModel.DataAnnotations;

namespace AgroControlAPI.Models
{
    public class Product
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
        public string? ProductType { get; set; }

        [MaxLength(50)]
        public string? ReleaseForm { get; set; }

        [MaxLength(10)]
        public string Unit { get; set; } = "кг";

        public string Status { get; set; } = "active";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}