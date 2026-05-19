using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgroControlAPI.Models;

namespace AgroControlAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductionOrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductionOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/productionorders
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _context.ProductionOrders
                .Include(o => o.Batches)
                .ToListAsync();
            return Ok(orders);
        }

        // GET: api/productionorders/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _context.ProductionOrders
                .Include(o => o.Batches)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound(new { message = "Заказ не найден" });
            return Ok(order);
        }

        // POST: api/productionorders
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductionOrder order)
        {
            order.OrderNumber = $"PO-{DateTime.Now:yyyyMMdd}-{new Random().Next(100, 999)}";
            order.CreatedAt = DateTime.UtcNow;
            order.Status = "planned";
            _context.ProductionOrders.Add(order);
            await _context.SaveChangesAsync();
            return Ok(new { id = order.Id, message = "Заказ создан", order });
        }

        // PUT: api/productionorders/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductionOrder order)
        {
            if (id != order.Id)
                return BadRequest(new { message = "ID не совпадают" });

            var existing = await _context.ProductionOrders.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = "Заказ не найден" });

            existing.RecipeId = order.RecipeId;
            existing.PlannedQuantityKg = order.PlannedQuantityKg;
            existing.Status = order.Status;
            existing.Priority = order.Priority;
            existing.PlannedStartDate = order.PlannedStartDate;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Заказ обновлен", order = existing });
        }

        // PUT: api/productionorders/{id}/start
        [HttpPut("{id}/start")]
        public async Task<IActionResult> Start(int id)
        {
            var order = await _context.ProductionOrders.FindAsync(id);
            if (order == null)
                return NotFound(new { message = "Заказ не найден" });

            order.Status = "in_progress";
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Заказ запущен в производство" });
        }

        // DELETE: api/productionorders/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.ProductionOrders.FindAsync(id);
            if (order == null)
                return NotFound(new { message = "Заказ не найден" });

            if (order.Status == "in_progress")
                return BadRequest(new { message = "Нельзя удалить заказ в работе" });

            _context.ProductionOrders.Remove(order);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Заказ удален" });
        }
    }
}