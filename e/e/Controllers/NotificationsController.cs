using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgroControlAPI.Models;

namespace AgroControlAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NotificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/notifications
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? unreadOnly = null)
        {
            var query = _context.Notifications.AsQueryable();

            if (unreadOnly == true)
                query = query.Where(n => !n.IsRead);

            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .Take(50)
                .ToListAsync();

            return Ok(notifications);
        }

        // GET: api/notifications/unread
        [HttpGet("unread")]
        public async Task<IActionResult> GetUnread()
        {
            var notifications = await _context.Notifications
                .Where(n => !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            return Ok(notifications);
        }

        // PUT: api/notifications/{id}/read
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
                return NotFound(new { message = "Уведомление не найдено" });

            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Уведомление отмечено как прочитанное" });
        }

        // PUT: api/notifications/read-all
        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            await _context.Notifications
                .Where(n => !n.IsRead)
                .ForEachAsync(n => n.IsRead = true);

            await _context.SaveChangesAsync();
            return Ok(new { message = "Все уведомления отмечены как прочитанные" });
        }

        // DELETE: api/notifications/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
                return NotFound(new { message = "Уведомление не найдено" });

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Уведомление удалено" });
        }
    }
}