using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgroControlAPI.Models;

namespace AgroControlAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BatchesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BatchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/batches
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var batches = await _context.Batches
                .Include(b => b.StepExecutions)
                .ToListAsync();
            return Ok(batches);
        }

        // GET: api/batches/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var batches = await _context.Batches
                .Where(b => b.Status == "running" || b.Status == "planned")
                .ToListAsync();
            return Ok(batches);
        }

        // GET: api/batches/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // ИСПРАВЛЕНО: сначала находим batch, потом загружаем связанные данные
            var batch = await _context.Batches
                .Include(b => b.StepExecutions)
                    .ThenInclude(se => se.ActualParameters)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (batch == null)
                return NotFound(new { message = "Партия не найдена" });
            return Ok(batch);
        }

        // POST: api/batches
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Batch batch)
        {
            // ИСПРАВЛЕНО: убираем Recipe, которого нет в модели
            var order = await _context.ProductionOrders.FindAsync(batch.OrderId);
            if (order == null)
                return BadRequest(new { message = "Заказ не найден" });

            batch.BatchNumber = $"B-{DateTime.Now:yyyyMMdd}-{new Random().Next(100, 999)}";
            batch.Status = "planned";
            batch.CreatedAt = DateTime.UtcNow;

            _context.Batches.Add(batch);
            await _context.SaveChangesAsync();
            return Ok(new { id = batch.Id, message = "Партия создана", batch });
        }

        // PUT: api/batches/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Batch batch)
        {
            if (id != batch.Id)
                return BadRequest(new { message = "ID не совпадают" });

            var existing = await _context.Batches.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = "Партия не найдена" });

            existing.OrderId = batch.OrderId;
            existing.Status = batch.Status;
            existing.LabStatus = batch.LabStatus;
            existing.ActualQuantityKg = batch.ActualQuantityKg;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Партия обновлена", batch = existing });
        }

        // PUT: api/batches/{id}/start
        [HttpPut("{id}/start")]
        public async Task<IActionResult> Start(int id)
        {
            var batch = await _context.Batches.FindAsync(id);
            if (batch == null)
                return NotFound(new { message = "Партия не найдена" });

            if (batch.Status != "planned")
                return BadRequest(new { message = "Партию можно запустить только из статуса 'planned'" });

            batch.Status = "running";
            batch.StartTime = DateTime.UtcNow;
            batch.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Создаем уведомление
            var notification = new Notification
            {
                Title = $"Запуск партии {batch.BatchNumber}",
                Message = $"Партия {batch.BatchNumber} запущена в производство",
                Type = "info",
                CreatedAt = DateTime.UtcNow
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Партия запущена" });
        }

        // PUT: api/batches/{id}/complete
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> Complete(int id)
        {
            // ИСПРАВЛЕНО: сначала находим batch, потом считаем шаги
            var batch = await _context.Batches
                .Include(b => b.StepExecutions)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (batch == null)
                return NotFound(new { message = "Партия не найдена" });

            var incompleteSteps = batch.StepExecutions
                .Count(se => se.Status != "completed" && se.Status != "skipped");

            if (incompleteSteps > 0)
                return BadRequest(new { message = $"Не все шаги выполнены. Осталось: {incompleteSteps}" });

            batch.Status = "completed";
            batch.EndTime = DateTime.UtcNow;
            batch.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Партия завершена" });
        }

        // PUT: api/batches/{id}/block
        [HttpPut("{id}/block")]
        public async Task<IActionResult> Block(int id, [FromBody] BlockRequest request)
        {
            var batch = await _context.Batches.FindAsync(id);
            if (batch == null)
                return NotFound(new { message = "Партия не найдена" });

            batch.Status = "blocked";
            batch.LabStatus = "blocked";
            batch.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var notification = new Notification
            {
                Title = $"Блокировка партии {batch.BatchNumber}",
                Message = $"Партия заблокирована. Причина: {request.Reason}",
                Type = "error",
                CreatedAt = DateTime.UtcNow
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Партия заблокирована" });
        }

        // DELETE: api/batches/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var batch = await _context.Batches.FindAsync(id);
            if (batch == null)
                return NotFound(new { message = "Партия не найдена" });

            if (batch.Status == "running")
                return BadRequest(new { message = "Нельзя удалить партию в работе" });

            _context.Batches.Remove(batch);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Партия удалена" });
        }
    }

    public class BlockRequest
    {
        public string Reason { get; set; } = string.Empty;
    }
}