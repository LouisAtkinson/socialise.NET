using api.Models;

namespace api.Interfaces
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetAllByRecipientIdAsync(string recipientId);

        Task<Notification?> GetByIdAsync(int id);

        Task<Notification?> DeleteAsync(int id);

        Task<Notification> AddAsync(Notification notification);

        Task<bool> NotificationExists(int id);

        Task MarkAsReadAsync(int id);
    }
}
