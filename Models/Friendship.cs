namespace api.Models
{
    public class Friendship
    {
        public int Id { get; set; }

        public string UserAId { get; set; }
        public User UserA { get; set; }

        public string UserBId { get; set; }
        public User UserB { get; set; }

        public FriendshipStatus Status { get; set; } = FriendshipStatus.Pending;
    }

    public enum FriendshipStatus
    {
        Pending,
        Accepted
    }
}