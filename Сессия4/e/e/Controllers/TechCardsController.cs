using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgroControlAPI.Models;

namespace AgroControlAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TechCardsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TechCardsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/techcards
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var techCards = await _context.TechCards
                .Include(t => t.Steps)
                .ToListAsync();
            return Ok(techCards);
        }

        // GET: api/techcards/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var techCard = await _context.TechCards
                .Include(t => t.Steps)
                .ThenInclude(s => s.Parameters)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (techCard == null)
                return NotFound(new { message = "Технологическая карта не найдена" });
            return Ok(techCard);
        }

        // POST: api/techcards
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TechCard techCard)
        {
            techCard.CreatedAt = DateTime.UtcNow;
            techCard.Status = "draft";
            _context.TechCards.Add(techCard);
            await _context.SaveChangesAsync();
            return Ok(new { id = techCard.Id, message = "Технологическая карта создана", techCard });
        }

        // PUT: api/techcards/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TechCard techCard)
        {
            if (id != techCard.Id)
                return BadRequest(new { message = "ID не совпадают" });

            var existing = await _context.TechCards.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = "Технологическая карта не найдена" });

            existing.ProductId = techCard.ProductId;
            existing.Version = techCard.Version;
            existing.Status = techCard.Status;
            existing.Description = techCard.Description;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Технологическая карта обновлена", techCard = existing });
        }

        // PUT: api/techcards/{id}/activate
        [HttpPut("{id}/activate")]
        public async Task<IActionResult> Activate(int id)
        {
            // Деактивируем все карты для этого продукта
            var techCard = await _context.TechCards.FindAsync(id);
            if (techCard == null)
                return NotFound(new { message = "Технологическая карта не найдена" });

            await _context.TechCards
                .Where(t => t.ProductId == techCard.ProductId && t.IsActive)
                .ForEachAsync(t => t.IsActive = false);

            techCard.IsActive = true;
            techCard.Status = "active";
            techCard.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Технологическая карта активирована" });
        }

        // DELETE: api/techcards/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var techCard = await _context.TechCards.FindAsync(id);
            if (techCard == null)
                return NotFound(new { message = "Технологическая карта не найдена" });

            techCard.Status = "archived";
            await _context.SaveChangesAsync();
            return Ok(new { message = "Технологическая карта архивирована" });
        }
    }
}