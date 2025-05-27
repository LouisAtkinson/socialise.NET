namespace api.Models
{
    public class Notification
    {
        public int Id { get; set; }  

        public int SenderId { get; set; }
        public User Sender { get; set; }

        public int RecipientId { get; set; }
        public User Recipient { get; set; } 

        public NotificationType Type { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public int? PostId { get; set; }
        public Post Post { get; set; }

        public int? CommentId { get; set; }
        public Comment Comment { get; set; }

        public int? DisplayPictureId { get; set; }
        public DisplayPicture DisplayPicture { get; set; }

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