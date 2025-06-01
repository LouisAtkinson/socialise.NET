namespace api.Models
{
    public class Friendship
    {
        public string Id { get; set; }

        public string UserAId { get; set; } = string.Empty;
        public User UserA { get; set; }

        public string UserBId { get; set; } = string.Empty;
        public User UserB { get; set; }

        public FriendshipStatus Status { get; set; } = FriendshipStatus.Pending;
    }

    public enum FriendshipStatus
    {
        Pending,
        Accepted
    }
}