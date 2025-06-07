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

        public static FriendshipStatusDto ToFriendshipStatusDto(
            this Friendship? friendship, 
            string loggedInUserId)
        {
            if (friendship == null)
            {
                return new FriendshipStatusDto
                {
                    AreFriends = false,
                    HasPendingRequestFromLoggedInUser = false,
                    HasPendingRequestFromOtherUser = false,
                    FriendshipId = null
                };
            }

            var areFriends = friendship.Status == FriendshipStatus.Accepted;

            bool hasPendingFromLoggedInUser = 
                friendship.Status == FriendshipStatus.Pending &&
                friendship.UserAId == loggedInUserId;

            bool hasPendingFromOtherUser =
                friendship.Status == FriendshipStatus.Pending &&
                friendship.UserBId == loggedInUserId;

            return new FriendshipStatusDto
            {
                AreFriends = areFriends,
                HasPendingRequestFromLoggedInUser = hasPendingFromLoggedInUser,
                HasPendingRequestFromOtherUser = hasPendingFromOtherUser,
                FriendshipId = friendship.Id
            };
        }
    }
}