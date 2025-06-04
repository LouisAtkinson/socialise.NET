using Microsoft.EntityFrameworkCore;
using api.Interfaces;
using api.Models;
using api.Data;
using api.Helpers;

namespace api.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Notification>> GetAllByRecipientIdAsync(string recipientId)
        {
            return await _context.Notifications
                .Where(n => n.RecipientId == recipientId)
                .OrderByDescending(n => n.Timestamp)
                .ToListAsync();
        }

        public Task<Notification> GetByIdAsync(int id)
        {
            return _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<Notification?> DeleteAsync(int id)
        {
            var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id);

            if (notification == null)
            {
                return null;
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task<Notification> AddAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task MarkAsReadAsync(int id)
        {
            var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id);

            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public Task<bool> NotificationExists(int id)
        {
            return _context.Notifications.AnyAsync(n => n.Id == id);
        }
    }
}