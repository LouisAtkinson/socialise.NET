using api.Models;

namespace api.Dtos
{
    public class NotificationDto
    {
        public string Id { get; set; }  

        public string SenderId { get; set; }
        public string RecipientId { get; set; }

        public NotificationTypeDto Type { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string? PostId { get; set; }
        public string? CommentId { get; set; }
        public string? DisplayPictureId { get; set; }

        public bool IsRead { get; set; } = false;
    }

    public enum NotificationTypeDto
    {
        FriendRequest,
        FriendRequestAccepted,
        PostComment,
        PostLike,
        CommentLike,
        DisplayPictureComment,
        DisplayPictureLike,
        DisplayPictureCommentLike,
        PostFromFriend
    }
}