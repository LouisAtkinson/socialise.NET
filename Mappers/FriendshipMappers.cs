using api.Dtos;
using api.Models;

namespace api.Mappers
{
    public static class FriendshipMappers
    {
        public static FriendshipDto ToFriendshipDto(this Friendship friendshipModel)
        {
            return new FriendshipDto
            {
                UserAId = friendshipModel.UserAId,
                UserAName = $"{friendshipModel.UserA.FirstName} {friendshipModel.UserA.LastName}",
                UserBId = friendshipModel.UserBId,
                UserBName = $"{friendshipModel.UserB.FirstName} {friendshipModel.UserB.LastName}",
                Status = friendshipModel.Status.ToString()
            };
        }
    }
}