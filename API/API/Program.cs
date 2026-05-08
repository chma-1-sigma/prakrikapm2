using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Добавляем сервисы
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Manufacturing API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and your token"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Authentication
var jwtSecret = builder.Configuration["JwtSettings:Secret"];
if (string.IsNullOrEmpty(jwtSecret))
{
    throw new Exception("JwtSettings:Secret is not configured in appsettings.json");
}

var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            var result = JsonSerializer.Serialize(new { message = "You are not authorized" });
            return context.Response.WriteAsync(result);
        }
    };
});

builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Регистрация сервисов
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IBatchService, BatchService>();

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Auto migrate database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        dbContext.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating the database.");
    }
}

app.Run();

// ========== MODELS ==========
public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Operator";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}

public class Product
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ProductionBatch
{
    public int Id { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public virtual Product? Product { get; set; }
    public decimal PlannedQuantity { get; set; }
    public decimal ActualQuantity { get; set; }
    public string Status { get; set; } = "Planned";
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }

    public static ApiResponse<T> Ok(T data, string message = "Success") => new()
    {
        Success = true,
        Message = message,
        Data = data
    };

    public static ApiResponse<T> Fail(string message, List<string>? errors = null) => new()
    {
        Success = false,
        Message = message,
        Errors = errors
    };
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductionBatch> ProductionBatches { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => u.Login).IsUnique();
        modelBuilder.Entity<Product>().HasIndex(p => p.Code).IsUnique();
        modelBuilder.Entity<ProductionBatch>().HasIndex(b => b.BatchNumber).IsUnique();

        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, FullName = "Administrator", Login = "admin", PasswordHash = HashPassword("admin123"), Role = "Admin", IsActive = true },
            new User { Id = 2, FullName = "John Technologist", Login = "technologist", PasswordHash = HashPassword("tech123"), Role = "Technologist", IsActive = true },
            new User { Id = 3, FullName = "Mary Laborant", Login = "laborant", PasswordHash = HashPassword("lab123"), Role = "Laborant", IsActive = true },
            new User { Id = 4, FullName = "Bob Operator", Login = "operator", PasswordHash = HashPassword("oper123"), Role = "Operator", IsActive = true }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Code = "PROD-001", Name = "Herbicide A", Description = "Selective herbicide for corn", IsActive = true },
            new Product { Id = 2, Code = "PROD-002", Name = "Insecticide B", Description = "Broad-spectrum insecticide", IsActive = true },
            new Product { Id = 3, Code = "PROD-003", Name = "Fungicide C", Description = "Systemic fungicide", IsActive = true }
        );
    }

    private static string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}

// ========== DTOs ==========
public class LoginDto
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterDto
{
    public string FullName { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "Operator";
}

public class CreateBatchDto
{
    public int ProductId { get; set; }
    public decimal PlannedQuantity { get; set; }
    public string? Notes { get; set; }
}

public class UpdateBatchDto
{
    public string? Status { get; set; }
    public decimal? ActualQuantity { get; set; }
    public string? Notes { get; set; }
}

// ========== SERVICES ==========
public interface IAuthService
{
    Task<User?> Authenticate(string login, string password);
    Task<bool> Register(RegisterDto dto);
    string GenerateToken(User user);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<User?> Authenticate(string login, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Login == login && u.IsActive);

        if (user == null) return null;

        var hashedPassword = HashPassword(password);
        return user.PasswordHash == hashedPassword ? user : null;
    }

    public async Task<bool> Register(RegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Login == dto.Login))
            return false;

        var user = new User
        {
            FullName = dto.FullName,
            Login = dto.Login,
            PasswordHash = HashPassword(dto.Password),
            Role = dto.Role,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_config["JwtSettings:Secret"]!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("FullName", user.FullName)
            }),
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}

public interface IProductService
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product product);
    Task<bool> UpdateAsync(int id, Product product);
    Task<bool> DeleteAsync(int id);
}

public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllAsync() => await _context.Products.ToListAsync();

    public async Task<Product?> GetByIdAsync(int id) => await _context.Products.FindAsync(id);

    public async Task<Product> CreateAsync(Product product)
    {
        product.CreatedAt = DateTime.UtcNow;
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<bool> UpdateAsync(int id, Product product)
    {
        var existing = await _context.Products.FindAsync(id);
        if (existing == null) return false;

        existing.Code = product.Code;
        existing.Name = product.Name;
        existing.Description = product.Description;
        existing.IsActive = product.IsActive;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        product.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }
}

public interface IBatchService
{
    Task<List<ProductionBatch>> GetAllAsync();
    Task<ProductionBatch?> GetByIdAsync(int id);
    Task<ProductionBatch> CreateAsync(CreateBatchDto dto);
    Task<bool> UpdateStatusAsync(int id, string status, decimal? actualQuantity = null);
    Task<List<ProductionBatch>> GetActiveAsync();
}

public class BatchService : IBatchService
{
    private readonly AppDbContext _context;

    public BatchService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductionBatch>> GetAllAsync() =>
        await _context.ProductionBatches.Include(b => b.Product).OrderByDescending(b => b.StartDate).ToListAsync();

    public async Task<ProductionBatch?> GetByIdAsync(int id) =>
        await _context.ProductionBatches.Include(b => b.Product).FirstOrDefaultAsync(b => b.Id == id);

    public async Task<ProductionBatch> CreateAsync(CreateBatchDto dto)
    {
        var product = await _context.Products.FindAsync(dto.ProductId);
        if (product == null) throw new Exception("Product not found");

        var batch = new ProductionBatch
        {
            BatchNumber = GenerateBatchNumber(),
            ProductId = dto.ProductId,
            PlannedQuantity = dto.PlannedQuantity,
            Status = "Planned",
            StartDate = DateTime.UtcNow,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow
        };

        _context.ProductionBatches.Add(batch);
        await _context.SaveChangesAsync();
        return batch;
    }

    public async Task<bool> UpdateStatusAsync(int id, string status, decimal? actualQuantity = null)
    {
        var batch = await _context.ProductionBatches.FindAsync(id);
        if (batch == null) return false;

        batch.Status = status;
        if (actualQuantity.HasValue) batch.ActualQuantity = actualQuantity.Value;
        if (status == "Completed") batch.EndDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<ProductionBatch>> GetActiveAsync() =>
        await _context.ProductionBatches
            .Include(b => b.Product)
            .Where(b => b.Status == "Planned" || b.Status == "InProgress")
            .ToListAsync();

    private static string GenerateBatchNumber() => $"BATCH-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
}

// ========== CONTROLLERS ==========

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (string.IsNullOrEmpty(dto.Login) || string.IsNullOrEmpty(dto.Password))
            return BadRequest(ApiResponse<object>.Fail("Login and password are required"));

        var user = await _authService.Authenticate(dto.Login, dto.Password);

        if (user == null)
            return Unauthorized(ApiResponse<object>.Fail("Invalid login or password"));

        var token = _authService.GenerateToken(user);

        return Ok(ApiResponse<object>.Ok(new
        {
            Token = token,
            User = new { user.Id, user.Login, user.FullName, user.Role }
        }, "Login successful"));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Invalid data"));

        var result = await _authService.Register(dto);

        if (!result)
            return BadRequest(ApiResponse<object>.Fail("User with this login already exists"));

        return Ok(ApiResponse<object>.Ok(null, "User registered successfully"));
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(ApiResponse<List<Product>>.Ok(products));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
            return NotFound(ApiResponse<object>.Fail($"Product with ID {id} not found"));

        return Ok(ApiResponse<Product>.Ok(product));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Technologist")]
    public async Task<IActionResult> Create([FromBody] Product product)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Invalid product data"));

        try
        {
            var created = await _productService.CreateAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<Product>.Ok(created, "Product created successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Technologist")]
    public async Task<IActionResult> Update(int id, [FromBody] Product product)
    {
        var result = await _productService.UpdateAsync(id, product);
        if (!result)
            return NotFound(ApiResponse<object>.Fail($"Product with ID {id} not found"));

        return Ok(ApiResponse<object>.Ok(null, "Product updated successfully"));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _productService.DeleteAsync(id);
        if (!result)
            return NotFound(ApiResponse<object>.Fail($"Product with ID {id} not found"));

        return Ok(ApiResponse<object>.Ok(null, "Product archived successfully"));
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BatchesController : ControllerBase
{
    private readonly IBatchService _batchService;

    public BatchesController(IBatchService batchService)
    {
        _batchService = batchService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var batches = await _batchService.GetAllAsync();
        return Ok(ApiResponse<List<ProductionBatch>>.Ok(batches));
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var batches = await _batchService.GetActiveAsync();
        return Ok(ApiResponse<List<ProductionBatch>>.Ok(batches));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var batch = await _batchService.GetByIdAsync(id);
        if (batch == null)
            return NotFound(ApiResponse<object>.Fail($"Batch with ID {id} not found"));

        return Ok(ApiResponse<ProductionBatch>.Ok(batch));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Technologist")]
    public async Task<IActionResult> Create([FromBody] CreateBatchDto dto)
    {
        if (dto.PlannedQuantity <= 0)
            return BadRequest(ApiResponse<object>.Fail("Planned quantity must be greater than 0"));

        try
        {
            var batch = await _batchService.CreateAsync(dto);
            return Ok(ApiResponse<ProductionBatch>.Ok(batch, "Batch created successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}/start")]
    [Authorize(Roles = "Admin,Technologist,Operator")]
    public async Task<IActionResult> Start(int id)
    {
        var result = await _batchService.UpdateStatusAsync(id, "InProgress");
        if (!result)
            return NotFound(ApiResponse<object>.Fail($"Batch with ID {id} not found"));

        return Ok(ApiResponse<object>.Ok(null, "Batch started successfully"));
    }

    [HttpPut("{id}/complete")]
    [Authorize(Roles = "Admin,Technologist,Operator")]
    public async Task<IActionResult> Complete(int id, [FromBody] decimal? actualQuantity = null)
    {
        var result = await _batchService.UpdateStatusAsync(id, "Completed", actualQuantity);
        if (!result)
            return NotFound(ApiResponse<object>.Fail($"Batch with ID {id} not found"));

        return Ok(ApiResponse<object>.Ok(null, "Batch completed successfully"));
    }

    [HttpPut("{id}/cancel")]
    [Authorize(Roles = "Admin,Technologist")]
    public async Task<IActionResult> Cancel(int id)
    {
        var result = await _batchService.UpdateStatusAsync(id, "Cancelled");
        if (!result)
            return NotFound(ApiResponse<object>.Fail($"Batch with ID {id} not found"));

        return Ok(ApiResponse<object>.Ok(null, "Batch cancelled successfully"));
    }
}
