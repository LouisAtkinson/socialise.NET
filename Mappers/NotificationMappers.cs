using api.Dtos;
using api.Models;

namespace api.Mappers
{
    public static class NotificationMappers
    {
        public static NotificationDto ToNotificationDto(this Notification notificationModel)
        {
            return new NotificationDto
            {
                Id = notificationModel.Id,
                SenderId = notificationModel.SenderId,
                RecipientId = notificationModel.RecipientId,
                Type = (NotificationTypeDto)notificationModel.Type,
                Timestamp = notificationModel.Timestamp,
                PostId = notificationModel.PostId,
                CommentId = notificationModel.CommentId,
                DisplayPictureId = notificationModel.DisplayPictureId,
                IsRead = notificationModel.IsRead
            };
        }
    }
}