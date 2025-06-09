using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using api.Data;
using api.Dtos;
using api.Models;
using api.Mappers;
using api.Interfaces;
using api.Extensions;

namespace api.Controllers
{
    [Route("api/friends")]
    [ApiController]
    public class FriendController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signinManager;
        private readonly INotificationRepository _notificationService;

        public FriendController(UserManager<User> userManager, ITokenService tokenService, SignInManager<User> signInManager, ApplicationDbContext context, INotificationRepository notificationService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signinManager = signInManager;
            _context = context;
            _notificationService = notificationService;
        }

        [HttpGet("all/{id}")]
        [Authorize]
        public async Task<IActionResult> GetFriends([FromRoute] string id)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == id);
            if (!userExists) return NotFound(new { error = "User not found." });

            var friendships = await _context.Friendships
                .Where(f => (f.UserAId == id || f.UserBId == id) && f.Status == FriendshipStatus.Accepted)
                .Select(f => f.UserAId == id ? f.UserB : f.UserA) 
                .ToListAsync();

            var friends = friendships.Select(friend => friend.ToUserMinimalDto()).ToList();

            return Ok(friends);
        }

        [HttpGet("status/{loggedInUserId}/{otherUserId}")]
        [Authorize]
        public async Task<IActionResult> GetFriendshipStatus(string loggedInUserId, string otherUserId)
        {
            var currentUserId = User.GetUserId();
            if (currentUserId != loggedInUserId) return Unauthorized();

            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f =>
                    (f.UserAId == loggedInUserId && f.UserBId == otherUserId) ||
                    (f.UserAId == otherUserId && f.UserBId == loggedInUserId));

            var dto = friendship.ToFriendshipStatusDto(loggedInUserId);

            return Ok(dto);
        }

        [HttpPost("request/{recipientUserId}")]
        [Authorize]
        public async Task<IActionResult> SendFriendRequest(string recipientUserId)
        {
            var senderUserId = User.GetUserId();
            if (string.IsNullOrEmpty(senderUserId)) return Unauthorized();

            var sender = await _userManager.FindByIdAsync(senderUserId);
            var recipient = await _userManager.FindByIdAsync(recipientUserId);

            if (recipient == null) return NotFound(new { message = "Recipient not found!" });

            var existingFriendship = await _context.Friendships.FirstOrDefaultAsync(f =>
                (f.UserAId == sender.Id && f.UserBId == recipient.Id) ||
                (f.UserAId == recipient.Id && f.UserBId == sender.Id)
            );

            if (existingFriendship != null)
                return BadRequest(new { message = "Friend request already exists or users are already friends!" });

            var newFriendship = new Friendship
            {
                UserAId = sender.Id,
                UserBId = recipient.Id,
                Status = FriendshipStatus.Pending
            };

            _context.Friendships.Add(newFriendship);
            await _context.SaveChangesAsync();

            var notification = new Notification
            {
                SenderId = sender.Id,
                RecipientId = recipient.Id,
                Type = NotificationType.friendRequest,
                Timestamp = DateTime.UtcNow
            };

            await _notificationService.AddAsync(notification);

            return Ok(new { message = "Friend request sent successfully!" });
        }

        [HttpPost("accept/{friendshipId}")]
        [Authorize]
        public async Task<IActionResult> AcceptFriendRequest(int friendshipId)
        {
            var currentUserId = User.GetUserId();
            if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

            var friendship = await _context.Friendships.FirstOrDefaultAsync(f =>
                f.Id == friendshipId && f.UserBId == currentUserId && f.Status == FriendshipStatus.Pending);

            if (friendship == null) return NotFound(new { error = "Friend request not found." });

            friendship.Status = FriendshipStatus.Accepted;

            var originalNotification = await _context.Notifications.FirstOrDefaultAsync(n =>
                n.RecipientId == currentUserId &&
                n.SenderId == friendship.UserAId &&
                n.Type == NotificationType.friendRequest);

            if (originalNotification != null)
            {
                _context.Notifications.Remove(originalNotification);
            }

            var notification = new Notification
            {
                SenderId = currentUserId,
                RecipientId = friendship.UserAId,
                Type = NotificationType.friendRequestAccepted,
                Timestamp = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Friend request accepted!" });
        }

        [HttpPost("reject/{friendshipId}")]
        [Authorize]
        public async Task<IActionResult> RejectFriendship(int friendshipId)
        {
            var currentUserId = User.GetUserId();
            if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f => f.Id == friendshipId &&
                    (f.UserBId == currentUserId || f.UserAId == currentUserId));

            if (friendship == null)
                return NotFound(new { error = "Friendship not found." });

            if (friendship.Status == FriendshipStatus.Pending && friendship.UserBId == currentUserId)
            {
                var originalNotification = await _context.Notifications.FirstOrDefaultAsync(n =>
                    n.RecipientId == currentUserId &&
                    n.SenderId == friendship.UserAId &&
                    n.Type == NotificationType.friendRequest);

                if (originalNotification != null)
                {
                    _context.Notifications.Remove(originalNotification);
                }

                _context.Friendships.Remove(friendship);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Friendship request rejected." });
            }

            return BadRequest(new { error = "Invalid operation." });
        }


        [HttpDelete("delete/{friendshipId}")]
        [Authorize]
        public async Task<IActionResult> DeleteFriendship(int friendshipId)
        {
            var currentUserId = User.GetUserId();
            if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f => f.Id == friendshipId &&
                    (f.UserAId == currentUserId || f.UserBId == currentUserId));

            if (friendship == null)
                return NotFound(new { error = "Friendship not found." });

            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Friendship deleted." });
        }
    }
}
