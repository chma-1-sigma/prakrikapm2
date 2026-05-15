//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using e.Models;

//namespace e.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class OrdersController : ControllerBase
//    {
//        private readonly ApplicationDbContext _context;

//        public OrdersController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetAll()
//        {
//            var orders = await _context.ProductionOrders.ToListAsync();
//            return Ok(orders);
//        }

//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetById(int id)
//        {
//            var order = await _context.ProductionOrders.FindAsync(id);
//            if (order == null) return NotFound();
//            return Ok(order);
//        }

//        [HttpPost]
//        public async Task<IActionResult> Create([FromBody] ProductionOrder order)
//        {
//            order.OrderNumber = $"PO-{DateTime.Now:yyyyMMdd}-{new Random().Next(100, 999)}";
//            order.CreatedAt = DateTime.UtcNow;
//            _context.ProductionOrders.Add(order);
//            await _context.SaveChangesAsync();
//            return Ok(new { id = order.Id, message = "Заказ создан" });
//        }

//        [HttpPut("{id}")]
//        public async Task<IActionResult> Update(int id, [FromBody] ProductionOrder order)
//        {
//            if (id != order.Id)
//                return BadRequest(new { message = "ID не совпадают" });

//            var existing = await _context.ProductionOrders.FindAsync(id);
//            if (existing == null)
//                return NotFound(new { message = "Заказ не найден" });

//            existing.RecipeId = order.RecipeId;
//            existing.PlannedQuantityKg = order.PlannedQuantityKg;
//            existing.Status = order.Status;
//            existing.Priority = order.Priority;
//            existing.PlannedStartDate = order.PlannedStartDate;
//            existing.UpdatedAt = DateTime.UtcNow;

//            await _context.SaveChangesAsync();
//            return Ok(new { message = "Заказ обновлен", order = existing });
//        }

//        [HttpPut("start/{id}")]
//        public async Task<IActionResult> Start(int id)
//        {
//            var order = await _context.ProductionOrders.FindAsync(id);
//            if (order == null) return NotFound();

//            order.Status = "in_progress";
//            order.UpdatedAt = DateTime.UtcNow;
//            await _context.SaveChangesAsync();

//            return Ok(new { message = "Заказ запущен" });
//        }

//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var order = await _context.ProductionOrders.FindAsync(id);
//            if (order == null) return NotFound();

//            _context.ProductionOrders.Remove(order);
//            await _context.SaveChangesAsync();
//            return Ok(new { message = "Заказ удален" });
//        }
//    }
//}