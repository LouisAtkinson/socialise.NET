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

        public FriendController(UserManager<User> userManager, ITokenService tokenService, SignInManager<User> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signinManager = signInManager;
            _context = context;
        }

        [HttpPost("request")]
        [Authorize]
        public async Task<IActionResult> SendFriendRequest(string recipientUserId)
        {
            var senderUserId = User.GetUserId();
            if (string.IsNullOrEmpty(senderUserId)) return Unauthorized();

            var sender = await _userManager.FindByIdAsync(senderUserId);
            var recipient = await _userManager.FindByIdAsync(recipientUserId);

            if (recipient == null) return NotFound("Recipient not found!");

            var existingFriendship = await _context.Friendships.FirstOrDefaultAsync(f =>
                (f.UserAId == sender.Id && f.UserBId == recipient.Id) ||
                (f.UserAId == recipient.Id && f.UserBId == sender.Id)
            );

            if (existingFriendship != null)
                return BadRequest("Friend request already exists or users are already friends!");

            var newFriendship = new Friendship
            {
                UserAId = sender.Id,
                UserBId = recipient.Id,
                Status = FriendshipStatus.Pending
            };

            _context.Friendships.Add(newFriendship);
            await _context.SaveChangesAsync();

            return Ok("Friend request sent successfully!");
        }

        [HttpPost("accept")]
        [Authorize]
        public async Task<IActionResult> AcceptFriendRequest(int friendshipId)
        {
            var currentUserId = User.GetUserId();
            if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

            var friendship = await _context.Friendships.FirstOrDefaultAsync(f =>
                f.Id == friendshipId && f.UserBId == currentUserId && f.Status == FriendshipStatus.Pending);

            if (friendship == null) return NotFound("Friend request not found!");

            friendship.Status = FriendshipStatus.Accepted;
            await _context.SaveChangesAsync();

            return Ok("Friend request accepted!");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetFriends()
        {
            var currentUserId = User.GetUserId();
            if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

            var friendships = await _context.Friendships
                .Include(f => f.UserA)
                .Include(f => f.UserB)
                .Where(f => (f.UserAId == currentUserId || f.UserBId == currentUserId) && f.Status == FriendshipStatus.Accepted)
                .ToListAsync();

            var friendDtos = friendships.Select(f => f.ToFriendshipDto());

            return Ok(friendDtos);
        }

        [HttpPost("reject")]
        [Authorize]
        public async Task<IActionResult> RejectFriendship(int friendshipId)
        {
            var currentUserId = User.GetUserId();
            if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f => f.Id == friendshipId &&
                    (f.UserBId == currentUserId || f.UserAId == currentUserId));

            if (friendship == null)
                return NotFound("Friendship not found.");

            if (friendship.Status == FriendshipStatus.Pending && friendship.UserBId == currentUserId)
            {
                _context.Friendships.Remove(friendship);
                await _context.SaveChangesAsync();
                return Ok("Friendship request rejected.");
            }

            return BadRequest("Invalid operation.");
        }

        [HttpDelete("delete")]
        [Authorize]
        public async Task<IActionResult> DeleteFriendship(int friendshipId)
        {
            var currentUserId = User.GetUserId();
            if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f => f.Id == friendshipId &&
                    (f.UserAId == currentUserId || f.UserBId == currentUserId));

            if (friendship == null)
                return NotFound("Friendship not found.");

            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();
            return Ok("Friendship deleted.");
        }
    }
}
