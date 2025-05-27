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
                Sender = notificationModel.Sender,
                RecipientId = notificationModel.RecipientId,
                Recipient = notificationModel.Recipient,
                Type = (NotificationTypeDto)notificationModel.Type,
                Timestamp = notificationModel.Timestamp,
                PostId = notificationModel.PostId,
                Post = notificationModel.Post,
                CommentId = notificationModel.CommentId,
                Comment = notificationModel.Comment,
                DisplayPictureId = notificationModel.DisplayPictureId,
                DisplayPicture = notificationModel.DisplayPicture,
                IsRead = notificationModel.IsRead
            };
        }
    }
}