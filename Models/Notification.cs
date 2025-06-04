namespace api.Models
{
    public class Notification
    {
        public int Id { get; set; }

        public string SenderId { get; set; }
        public string RecipientId { get; set; }

        public NotificationType Type { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public int? PostId { get; set; }
        public int? CommentId { get; set; }
        public int? DisplayPictureId { get; set; }

        public bool IsRead { get; set; } = false;
    }

    public enum NotificationType
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