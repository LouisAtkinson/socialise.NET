using api.Models;

namespace api.Dtos
{
    public class FriendshipDto
    {
        public string UserAId { get; set; }
        public string UserAName { get; set; }

        public string UserBId { get; set; }
        public string UserBName { get; set; }

        public string Status { get; set; }
    }
}