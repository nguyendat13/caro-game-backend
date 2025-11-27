using backend.DTOs.Notification;

namespace backend.Services
{
    public interface INotificationService
    {
        Task<List<NotificationDTO>> GetNotificationsForUserAsync(int userId);
        Task<bool> DeleteNotificationAsync(int notificationId, int userId);
        Task<bool> MarkAsReadAsync(int notificationId, int userId);
    }
}
