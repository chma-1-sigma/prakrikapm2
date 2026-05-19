using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgroControlAPI.Models;

namespace AgroControlAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RecipesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/recipes
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var recipes = await _context.Recipes
                .Include(r => r.Components)
                .ToListAsync();
            return Ok(recipes);
        }

        // GET: api/recipes/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.Components)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null)
                return NotFound(new { message = "Рецептура не найдена" });
            return Ok(recipe);
        }

        // POST: api/recipes
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Recipe recipe)
        {
            recipe.CreatedAt = DateTime.UtcNow;
            recipe.Status = "draft";
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
            return Ok(new { id = recipe.Id, message = "Рецептура создана", recipe });
        }

        // PUT: api/recipes/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Recipe recipe)
        {
            if (id != recipe.Id)
                return BadRequest(new { message = "ID не совпадают" });

            var existing = await _context.Recipes.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = "Рецептура не найдена" });

            existing.ProductId = recipe.ProductId;
            existing.Version = recipe.Version;
            existing.Status = recipe.Status;
            existing.Description = recipe.Description;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Рецептура обновлена", recipe = existing });
        }

        // PUT: api/recipes/{id}/approve
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
                return NotFound(new { message = "Рецептура не найдена" });

            // Проверка суммы компонентов = 100%
            var components = await _context.RecipeComponents
                .Where(c => c.RecipeId == id)
                .ToListAsync();

            if (components.Any())
            {
                var total = components.Sum(c => c.QuantityPercent);
                if (Math.Abs(total - 100) > 0.01m)
                    return BadRequest(new { message = $"Сумма компонентов должна быть 100%. Текущая: {total}%" });
            }

            recipe.Status = "active";
            recipe.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Рецептура утверждена" });
        }

        // DELETE: api/recipes/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
                return NotFound(new { message = "Рецептура не найдена" });

            recipe.Status = "archived";
            await _context.SaveChangesAsync();
            return Ok(new { message = "Рецептура архивирована" });
        }
    }
}