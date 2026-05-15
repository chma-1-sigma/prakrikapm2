using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgroControlAPI.Models;

namespace AgroControlAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RawMaterialsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RawMaterialsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/rawmaterials
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var materials = await _context.RawMaterials.ToListAsync();
            return Ok(materials);
        }

        // GET: api/rawmaterials/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var material = await _context.RawMaterials.FindAsync(id);
            if (material == null)
                return NotFound(new { message = "Сырье не найдено" });
            return Ok(material);
        }

        // POST: api/rawmaterials
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RawMaterial material)
        {
            if (await _context.RawMaterials.AnyAsync(r => r.Code == material.Code))
                return BadRequest(new { message = "Сырье с таким кодом уже существует" });

            material.CreatedAt = DateTime.UtcNow;
            _context.RawMaterials.Add(material);
            await _context.SaveChangesAsync();
            return Ok(new { id = material.Id, message = "Сырье добавлено", material });
        }

        // PUT: api/rawmaterials/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RawMaterial material)
        {
            if (id != material.Id)
                return BadRequest(new { message = "ID не совпадают" });

            var existing = await _context.RawMaterials.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = "Сырье не найдено" });

            existing.Code = material.Code;
            existing.Name = material.Name;
            existing.Category = material.Category;
            existing.Unit = material.Unit;
            existing.Supplier = material.Supplier;
            existing.IsActive = material.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Сырье обновлено", material = existing });
        }

        // DELETE: api/rawmaterials/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var material = await _context.RawMaterials.FindAsync(id);
            if (material == null)
                return NotFound(new { message = "Сырье не найдено" });

            material.IsActive = false;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Сырье деактивировано" });
        }
    }
}