using Microsoft.EntityFrameworkCore;

namespace AgroControlAPI.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<RawMaterial> RawMaterials { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeComponent> RecipeComponents { get; set; }
        public DbSet<TechCard> TechCards { get; set; }
        public DbSet<TechCardStep> TechCardSteps { get; set; }
        public DbSet<StepParameter> StepParameters { get; set; }
        public DbSet<ProductionOrder> ProductionOrders { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<BatchStepExecution> BatchStepExecutions { get; set; }
        public DbSet<StepActualParameter> StepActualParameters { get; set; }
        public DbSet<QualityControl> QualityControls { get; set; }
        public DbSet<Deviation> Deviations { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Уникальные индексы
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Product>().HasIndex(p => p.Code).IsUnique();
            modelBuilder.Entity<RawMaterial>().HasIndex(r => r.Code).IsUnique();
            modelBuilder.Entity<Batch>().HasIndex(b => b.BatchNumber).IsUnique();
            modelBuilder.Entity<ProductionOrder>().HasIndex(o => o.OrderNumber).IsUnique();

            // Ограничение: только одна активная рецептура на продукт
            modelBuilder.Entity<Recipe>()
                .HasIndex(r => new { r.ProductId, r.Status })
                .HasFilter("[Status] = 'active'")
                .IsUnique();

            // Ограничение: только одна активная технологическая карта на продукт
            modelBuilder.Entity<TechCard>()
                .HasIndex(t => new { t.ProductId, t.IsActive })
                .HasFilter("[IsActive] = 1")
                .IsUnique();

            // Настройка decimal
            modelBuilder.Entity<RecipeComponent>()
                .Property(r => r.QuantityPercent)
                .HasPrecision(18, 4);

            modelBuilder.Entity<StepParameter>()
                .Property(s => s.PlannedValue)
                .HasPrecision(18, 2);

            modelBuilder.Entity<StepActualParameter>()
                .Property(s => s.ActualValue)
                .HasPrecision(18, 2);

            modelBuilder.Entity<QualityControl>()
                .Property(q => q.MeasuredValue)
                .HasPrecision(18, 2);
        }
    }
}