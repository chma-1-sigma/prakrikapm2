//using System.ComponentModel.DataAnnotations;
//using System.Text.Json.Serialization;
//using Microsoft.EntityFrameworkCore;

//namespace e.Models
//{
//    // =====================================================
//    // ПОЛЬЗОВАТЕЛИ
//    // =====================================================

//    public class User
//    {
//        [Key]
//        public int Id { get; set; }

//        [Required]
//        [MaxLength(50)]
//        public string Username { get; set; } = string.Empty;

//        [Required]
//        [MaxLength(255)]
//        [JsonIgnore]
//        public string PasswordHash { get; set; } = string.Empty;

//        [Required]
//        [MaxLength(150)]
//        public string FullName { get; set; } = string.Empty;

//        [Required]
//        [MaxLength(50)]
//        public string Role { get; set; } = string.Empty;

//        [EmailAddress]
//        [MaxLength(100)]
//        public string Email { get; set; } = string.Empty;

//        [Phone]
//        [MaxLength(20)]
//        public string? Phone { get; set; }

//        public bool IsActive { get; set; } = true;

//        public DateTime? LastLogin { get; set; }

//        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

//        [MaxLength(100)]
//        public string? Department { get; set; }

//        public int FailedLoginAttempts { get; set; } = 0;

//        public DateTime? LockedUntil { get; set; }
//    }

//    // =====================================================
//    // ПРОДУКТЫ
//    // =====================================================

//    public class Product
//    {
//        [Key]
//        public int Id { get; set; }

//        [Required]
//        [MaxLength(20)]
//        public string Code { get; set; } = string.Empty;

//        [Required]
//        [MaxLength(150)]
//        public string Name { get; set; } = string.Empty;

//        [MaxLength(50)]
//        public string? ProductType { get; set; }

//        [MaxLength(50)]
//        public string? ReleaseForm { get; set; }

//        [MaxLength(10)]
//        public string Unit { get; set; } = "кг";

//        public string Status { get; set; } = "active";

//        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
//        public DateTime? UpdatedAt { get; set; }

//        public int? CreatedBy { get; set; }
//    }

//    // =====================================================
//    // СЫРЬЕ
//    // =====================================================

//    public class RawMaterial
//    {
//        [Key]
//        public int Id { get; set; }

//        [Required]
//        [MaxLength(20)]
//        public string Code { get; set; } = string.Empty;

//        [Required]
//        [MaxLength(150)]
//        public string Name { get; set; } = string.Empty;

//        [MaxLength(50)]
//        public string? Category { get; set; }

//        [MaxLength(10)]
//        public string? Unit { get; set; }

//        [MaxLength(100)]
//        public string? Supplier { get; set; }

//        public bool IsActive { get; set; } = true;

//        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
//        public DateTime? UpdatedAt { get; set; }
//    }

//    // =====================================================
//    // РЕЦЕПТУРЫ
//    // =====================================================

//    public class Recipe
//    {
//        [Key]
//        public int Id { get; set; }

//        [Required]
//        public int ProductId { get; set; }
//        public virtual Product? Product { get; set; }

//        public int Version { get; set; } = 1;

//        [MaxLength(20)]
//        public string Status { get; set; } = "draft"; // draft, active, archived

//        public int? CreatedBy { get; set; }
//        public int? ApprovedBy { get; set; }

//        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
//        public DateTime? ApprovedAt { get; set; }
//        public DateTime? UpdatedAt { get; set; }

//        public string? Description { get; set; }
//        public string? Notes { get; set; }

//        public virtual ICollection<RecipeComponent> Components { get; set; } = new List<RecipeComponent>();
//    }

//    // =====================================================
//    // КОМПОНЕНТЫ РЕЦЕПТУРЫ (ИСПРАВЛЕННЫЕ - ДОБАВЛЕНЫ ToleranceMin/ToleranceMax)
//    // =====================================================

//    public class RecipeComponent
//    {
//        [Key]
//        public int Id { get; set; }

//        [Required]
//        public int RecipeId { get; set; }
//        public virtual Recipe? Recipe { get; set; }

//        [Required]
//        public int RawMaterialId { get; set; }
//        public virtual RawMaterial? RawMaterial { get; set; }

//        [Range(0.01, 100)]
//        public decimal QuantityPercent { get; set; }

//        public int LoadOrder { get; set; }

//        public decimal ToleranceMin { get; set; } = 0;      // <--- ДОБАВЛЕНО
//        public decimal ToleranceMax { get; set; } = 0;      // <--- ДОБАВЛЕНО

//        public bool IsCritical { get; set; } = false;

//        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
//    }

//    // =====================================================
//    // ТЕХНОЛОГИЧЕСКИЕ КАРТЫ
//    // =====================================================

//    public class TechCard
//    {
//        [Key]
//        public int Id { get; set; }

//        [Required]
//        public int ProductId { get; set; }
//        public virtual Product? Product { get; set; }

//        public int Version { get; set; } = 1;

//        [MaxLength(20)]
//        public string Status { get; set; } = "draft";

//        public bool IsActive { get; set; } = false;

//        public int? CreatedBy { get; set; }
//        public int? ApprovedBy { get; set; }

//        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
//        public DateTime? ApprovedAt { get; set; }

//        public string? Description { get; set; }

//        public virtual ICollection<TechCardStep> Steps { get; set; } = new List<TechCardStep>();
//    }

//    // =====================================================
//    // ШАГИ ТЕХНОЛОГИЧЕСКОЙ КАРТЫ
//    // =====================================================

//    public class TechCardStep
//    {
//        [Key]
//        public int Id { get; set; }

//        [Required]
//        public int TechCardId { get; set; }
//        public virtual TechCard? TechCard { get; set; }

//        public int StepOrder { get; set; }

//        [Required]
//        [MaxLength(100)]
//        public string StepName { get; set; } = string.Empty;

//        [MaxLength(50)]
//        public string? StepType { get; set; }

//        public int? EquipmentId { get; set; }

//        public int? PlannedDurationMin { get; set; }

//        public bool IsMandatory { get; set; } = true;

//        public string? Instruction { get; set; }

//        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

//        public virtual ICollection<StepParameter> Parameters { get; set; } = new List<StepParameter>();
//    }

//    // =====================================================
//    // ПАРАМЕТРЫ ШАГА
//    // =====================================================

//    public class StepParameter
//    {
//        [Key]
//        public int Id { get; set; }

//        [Required]
//        public int StepId { get; set; }
//        public virtual TechCardStep? Step { get; set; }

//        [Required]
//        [MaxLength(50)]
//        public string ParameterName { get; set; } = string.Empty;

//        public decimal? PlannedValue { get; set; }

//        [MaxLength(10)]
//        public string? Unit { get; set; }

//        public decimal? ToleranceMin { get; set; }
//        public decimal? ToleranceMax { get; set; }

//        public bool IsCritical { get; set; } = false;
//    }

//    // =====================================================
//    // ПРОИЗВОДСТВЕННЫЕ ЗАКАЗЫ
//    // =====================================================

//    public class ProductionOrder
//    {
//        [Key]
//        public int Id { get; set; }

//        [Required]
//        [MaxLength(20)]
//        public string OrderNumber { get; set; } = string.Empty;

//        [Required]
//        public int RecipeId { get; set; }
//        public virtual Recipe? Recipe { get; set; }

//        public int PlannedQuantityKg { get; set; }

//        [MaxLength(20)]
//        public string Status { get; set; } = "planned";

//        public int Priority { get; set; } = 1;

//        public DateTime? PlannedStartDate { get; set; }
//        public DateTime? PlannedEndDate { get; set; }

//        public int? CreatedBy { get; set; }

//        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
//        public DateTime? UpdatedAt { get; set; }

//        public virtual ICollection<Batch> Batches { get; set; } = new List<Batch>();
//    }

//    // =====================================================
//    // ПРОИЗВОДСТВЕННЫЕ ПАРТИИ
//    // =====================================================

//    public class Batch
//    {
//        [Key]
//        public int Id { get; set; }

//        [Required]
//        [MaxLength(20)]
//        public string BatchNumber { get; set; } = string.Empty;

//        [Required]
//        public int OrderId { get; set; }
//        public virtual ProductionOrder? Order { get; set; }

//        [Required]
//        public int ProductId { get; set; }
//        public virtual Product? Product { get; set; }

//        public int? RecipeVersion { get; set; }

//        public DateTime? StartTime { get; set; }
//        public DateTime? EndTime { get; set; }

//        [MaxLength(20)]
//        public string Status { get; set; } = "planned";

//        public int ActualQuantityKg { get; set; } = 0;

//        [MaxLength(20)]
//        public string LabStatus { get; set; } = "pending";

//        public int? ShiftSupervisorId { get; set; }

//        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
//        public DateTime? UpdatedAt { get; set; }

//        public virtual ICollection<BatchStepExecution> StepExecutions { get; set; } = new List<BatchStepExecution>();
//        public virtual ICollection<QualityControl> QualityControls { get; set; } = new List<QualityControl>();
//    }

//    // =====================================================
//    // ВЫПОЛНЕНИЕ ШАГОВ
//    // =====================================================

//    public class BatchStepExecution
//    {
//        [Key]
//        public int Id { get; set; }

//        [Required]
//        public int BatchId { get; set; }
//        public virtual Batch? Batch { get; set; }

//        [Required]
//        public int StepId { get; set; }
//        public virtual TechCardStep? Step { get; set; }

//        public int? EquipmentId { get; set; }

//        public DateTime? StartTime { get; set; }
//        public DateTime? EndTime { get; set; }

//        [MaxLength(20)]
//        public string Status { get; set; } = "pending";

//        public int? OperatorId { get; set; }

//        public bool DeviationFlag { get; set; } = false;

//        public string? OperatorComment { get; set; }

//        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
//        public DateTime? UpdatedAt { get; set; }

//        public virtual ICollection<StepActualParameter> ActualParameters { get; set; } = new List<StepActualParameter>();
//    }

//    // =====================================================
//    // ФАКТИЧЕСКИЕ ПАРАМЕТРЫ
//    // =====================================================

//    public class StepActualParameter
//    {
//        [Key]
//        public int Id { get; set; }

//        [Required]
//        public int StepExecutionId { get; set; }
//        public virtual BatchStepExecution? StepExecution { get; set; }

//        [Required]
//        [MaxLength(50)]
//        public string ParameterName { get; set; } = string.Empty;

//        public decimal? ActualValue { get; set; }

//        [MaxLength(10)]
//        public string? Unit { get; set; }

//        public bool IsDeviated { get; set; } = false;

//        public string? DeviationReason { get; set; }
//    }

//    // =====================================================
//    // КОНТРОЛЬ КАЧЕСТВА
//    // =====================================================

//    public class QualityControl
//    {
//        [Key]
//        public int Id { get; set; }

//        [Required]
//        public int BatchId { get; set; }
//        public virtual Batch? Batch { get; set; }

//        [Required]
//        [MaxLength(20)]
//        public string SampleType { get; set; } = string.Empty;

//        [Required]
//        [MaxLength(100)]
//        public string ParameterName { get; set; } = string.Empty;

//        public decimal? MeasuredValue { get; set; }

//        [MaxLength(50)]
//        public string? StandardValue { get; set; }

//        [MaxLength(10)]
//        public string? Unit { get; set; }

//        [MaxLength(20)]
//        public string? Result { get; set; }

//        [MaxLength(20)]
//        public string? Decision { get; set; }

//        public int? AnalystId { get; set; }

//        public DateTime AnalysisDate { get; set; } = DateTime.UtcNow;

//        public string? Comment { get; set; }
//    }

//    // =====================================================
//    // ОТКЛОНЕНИЯ
//    // =====================================================

//    public class Deviation
//    {
//        [Key]
//        public int Id { get; set; }

//        public int? BatchId { get; set; }
//        public virtual Batch? Batch { get; set; }

//        public int? StepExecutionId { get; set; }
//        public virtual BatchStepExecution? StepExecution { get; set; }

//        [MaxLength(50)]
//        public string? ParameterName { get; set; }

//        public string? PlannedValue { get; set; }
//        public string? ActualValue { get; set; }

//        [MaxLength(20)]
//        public string Severity { get; set; } = "warning";

//        public string? Description { get; set; }

//        public int? ReportedBy { get; set; }

//        public DateTime ReportedAt { get; set; } = DateTime.UtcNow;

//        public DateTime? ResolvedAt { get; set; }
//        public int? ResolvedBy { get; set; }

//        public string? ResolutionComment { get; set; }
//    }

//    // =====================================================
//    // УВЕДОМЛЕНИЯ
//    // =====================================================

//    public class Notification
//    {
//        [Key]
//        public int Id { get; set; }

//        public int? UserId { get; set; }
//        public virtual User? User { get; set; }

//        [Required]
//        [MaxLength(200)]
//        public string Title { get; set; } = string.Empty;

//        [Required]
//        public string Message { get; set; } = string.Empty;

//        [MaxLength(50)]
//        public string Type { get; set; } = string.Empty;

//        public bool IsRead { get; set; } = false;

//        public int? RelatedEntityId { get; set; }
//        public string? RelatedEntityType { get; set; }

//        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
//    }

//    // =====================================================
//    // КОНТЕКСТ БАЗЫ ДАННЫХ
//    // =====================================================

//    public class ApplicationDbContext : DbContext
//    {
//        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
//            : base(options)
//        {
//        }

//        public DbSet<User> Users { get; set; }
//        public DbSet<Product> Products { get; set; }
//        public DbSet<RawMaterial> RawMaterials { get; set; }
//        public DbSet<Recipe> Recipes { get; set; }
//        public DbSet<RecipeComponent> RecipeComponents { get; set; }
//        public DbSet<TechCard> TechCards { get; set; }
//        public DbSet<TechCardStep> TechCardSteps { get; set; }
//        public DbSet<StepParameter> StepParameters { get; set; }
//        public DbSet<ProductionOrder> ProductionOrders { get; set; }
//        public DbSet<Batch> Batches { get; set; }
//        public DbSet<BatchStepExecution> BatchStepExecutions { get; set; }
//        public DbSet<StepActualParameter> StepActualParameters { get; set; }
//        public DbSet<QualityControl> QualityControls { get; set; }
//        public DbSet<Deviation> Deviations { get; set; }
//        public DbSet<Notification> Notifications { get; set; }

//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            base.OnModelCreating(modelBuilder);

//            // Уникальные индексы
//            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
//            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
//            modelBuilder.Entity<Product>().HasIndex(p => p.Code).IsUnique();
//            modelBuilder.Entity<RawMaterial>().HasIndex(r => r.Code).IsUnique();
//            modelBuilder.Entity<Batch>().HasIndex(b => b.BatchNumber).IsUnique();
//            modelBuilder.Entity<ProductionOrder>().HasIndex(o => o.OrderNumber).IsUnique();

//            // Только одна активная рецептура на продукт
//            modelBuilder.Entity<Recipe>()
//                .HasIndex(r => new { r.ProductId, r.Status })
//                .HasFilter("[Status] = 'active'")
//                .IsUnique();
//        }
//    }
//}