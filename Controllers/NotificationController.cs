using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using api.Data;
using api.Dtos;
using api.Models;
using api.Mappers;

namespace api.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NotificationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetNotifications(string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.RecipientId == userId)
                .ToListAsync();

            var notificationDtos = notifications.Select(n => n.ToNotificationDto()).ToList();

            return Ok(notificationDtos);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(string id)
        {
            var notification = await _context.Notifications.FindAsync(id);

            if (notification == null)
            {
                return NotFound();
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("all/{userId}")]
        public async Task<IActionResult> DeleteAllNotifications(string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.RecipientId == userId)
                .ToListAsync();

            if (!notifications.Any())
            {
                return NotFound();
            }

            _context.Notifications.RemoveRange(notifications);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("mark-as-read/{id}")]
        public async Task<IActionResult> MarkAsRead(string id)
        {
            var notification = await _context.Notifications.FindAsync(id);

            if (notification == null)
            {
                return NotFound();
            }

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("mark-all-as-read/{userId}")]
        public async Task<IActionResult> MarkAllAsRead(string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.RecipientId == userId && !n.IsRead)
                .ToListAsync();

            if (!notifications.Any())
            {
                return NotFound();
            }

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
