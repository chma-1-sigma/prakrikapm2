using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgroControlAPI.Models;

namespace AgroControlAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipeComponentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RecipeComponentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/recipecomponents/recipe/{recipeId}
        [HttpGet("recipe/{recipeId}")]
        public async Task<IActionResult> GetByRecipeId(int recipeId)
        {
            var components = await _context.RecipeComponents
                .Where(c => c.RecipeId == recipeId)
                .OrderBy(c => c.LoadOrder)
                .ToListAsync();
            return Ok(components);
        }

        // POST: api/recipecomponents
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RecipeComponent component)
        {
            _context.RecipeComponents.Add(component);
            await _context.SaveChangesAsync();
            return Ok(new { id = component.Id, message = "Компонент добавлен", component });
        }

        // PUT: api/recipecomponents/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RecipeComponent component)
        {
            if (id != component.Id)
                return BadRequest(new { message = "ID не совпадают" });

            var existing = await _context.RecipeComponents.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = "Компонент не найден" });

            existing.RawMaterialId = component.RawMaterialId;
            existing.QuantityPercent = component.QuantityPercent;
            existing.LoadOrder = component.LoadOrder;
            existing.ToleranceMin = component.ToleranceMin;
            existing.ToleranceMax = component.ToleranceMax;
            existing.IsCritical = component.IsCritical;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Компонент обновлен", component = existing });
        }

        // DELETE: api/recipecomponents/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var component = await _context.RecipeComponents.FindAsync(id);
            if (component == null)
                return NotFound(new { message = "Компонент не найден" });

            _context.RecipeComponents.Remove(component);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Компонент удален" });
        }
    }
}