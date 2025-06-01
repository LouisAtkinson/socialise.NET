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

        [HttpPost("friends/request")]
        [Authorize]
        public async Task<IActionResult> SendFriendRequest(string recipientEmail)
        {
            var senderEmail = User.GetUserEmail();
            var sender = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == senderEmail.ToLower());
            var recipient = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == recipientEmail.ToLower());

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

        [HttpPost("friends/accept")]
        [Authorize]
        public async Task<IActionResult> AcceptFriendRequest(string friendshipId)
        {
            var email = User.GetUserEmail();
            var currentUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email.ToLower());

            var friendship = await _context.Friendships.FirstOrDefaultAsync(f =>
                f.Id == friendshipId && f.UserBId == currentUser.Id && f.Status == FriendshipStatus.Pending);

            if (friendship == null) return NotFound("Friend request not found!");

            friendship.Status = FriendshipStatus.Accepted;

            await _context.SaveChangesAsync();

            return Ok("Friend request accepted!");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetFriends()
        {
            var email = User.GetUserEmail();
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var currentUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            if (currentUser == null) return Unauthorized();

            var friendships = await _context.Friendships
                .Include(f => f.UserA)
                .Include(f => f.UserB)
                .Where(f => (f.UserAId == currentUser.Id || f.UserBId == currentUser.Id) && f.Status == FriendshipStatus.Accepted)
                .ToListAsync();

            var friendDtos = friendships.Select(f => f.ToFriendshipDto());

            return Ok(friendDtos);
        }

        [HttpPost("reject")]
        public async Task<IActionResult> RejectFriendship(string friendshipId)
        {
            var email = User.GetUserEmail();
            var currentUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email.ToLower());
            var userId = currentUser?.Id;

            if (userId == null) return Unauthorized();

            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f => f.Id == friendshipId &&
                    (f.UserBId == userId || f.UserAId == userId));

            if (friendship == null)
                return NotFound("Friendship not found.");

            if (friendship.Status == FriendshipStatus.Pending && friendship.UserBId == userId)
            {
                _context.Friendships.Remove(friendship);
                await _context.SaveChangesAsync();
                return Ok("Friendship request rejected.");
            }

            return BadRequest("Invalid operation.");
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteFriendship(string friendshipId)
        {
            var email = User.GetUserEmail();
            var currentUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email.ToLower());
            var userId = currentUser?.Id;

            if (userId == null) return Unauthorized();

            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f => f.Id == friendshipId &&
                    (f.UserAId == userId || f.UserBId == userId));

            if (friendship == null)
                return NotFound("Friendship not found.");

            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();
            return Ok("Friendship deleted.");
        }
    }
}