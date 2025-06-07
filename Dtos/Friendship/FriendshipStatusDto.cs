namespace api.Dtos
{
    public class FriendshipStatusDto
    {
        public bool AreFriends { get; set; }
        public bool HasPendingRequestFromLoggedInUser { get; set; }
        public bool HasPendingRequestFromOtherUser { get; set; }
        public int? FriendshipId { get; set; } 
    }
}