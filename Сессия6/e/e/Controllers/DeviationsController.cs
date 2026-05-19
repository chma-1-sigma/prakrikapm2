using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgroControlAPI.Models;

namespace AgroControlAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DeviationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/deviations
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? batchId = null,
            [FromQuery] string? severity = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            var query = _context.Deviations.AsQueryable();

            if (batchId.HasValue)
                query = query.Where(d => d.BatchId == batchId);

            if (!string.IsNullOrEmpty(severity))
                query = query.Where(d => d.Severity == severity);

            if (from.HasValue)
                query = query.Where(d => d.ReportedAt >= from);

            if (to.HasValue)
                query = query.Where(d => d.ReportedAt <= to);

            var deviations = await query
                .OrderByDescending(d => d.ReportedAt)
                .ToListAsync();

            return Ok(deviations);
        }

        // GET: api/deviations/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var deviation = await _context.Deviations.FindAsync(id);
            if (deviation == null)
                return NotFound(new { message = "Отклонение не найдено" });
            return Ok(deviation);
        }

        // PUT: api/deviations/{id}/resolve
        [HttpPut("{id}/resolve")]
        public async Task<IActionResult> Resolve(int id, [FromBody] ResolveDeviationRequest request)
        {
            var deviation = await _context.Deviations.FindAsync(id);
            if (deviation == null)
                return NotFound(new { message = "Отклонение не найдено" });

            deviation.ResolvedAt = DateTime.UtcNow;
            deviation.ResolutionComment = request.Comment;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Отклонение устранено" });
        }
    }

    public class ResolveDeviationRequest
    {
        public string Comment { get; set; } = string.Empty;
    }
}