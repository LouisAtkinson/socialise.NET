using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using api.Data;
using api.Dtos;
using api.Models;
using api.Mappers;
using api.Interfaces;
using api.Extensions;

namespace api.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public PostController(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("all")]
        [Authorize]
        public async Task<IActionResult> GetAllPosts()
        {
            var email = User.GetUserEmail();
            var currentUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (currentUser == null) return Unauthorized("User not found.");

            var userFriends = await _context.Friendships
                .Where(f => (f.UserAId == currentUser.Id || f.UserBId == currentUser.Id) && f.Status == FriendshipStatus.Accepted)
                .Select(f => f.UserAId == currentUser.Id ? f.UserBId : f.UserAId)
                .ToListAsync();

            var posts = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Recipient)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .Where(p =>
                    p.AuthorId == currentUser.Id ||
                    (userFriends.Contains(p.AuthorId) && p.RecipientId == null) ||
                    (userFriends.Contains(p.AuthorId) || userFriends.Contains(p.RecipientId)))
                .ToListAsync();

            return Ok(posts.Select(p => p.ToPostDto()));
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUserPosts(string userId)
        {
            var email = User.GetUserEmail();
            var currentUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (currentUser == null) return Unauthorized("User not found.");

            var isFriend = await _context.Friendships.AnyAsync(f =>
                f.Status == FriendshipStatus.Accepted &&
                ((f.UserAId == currentUser.Id && f.UserBId == userId) ||
                 (f.UserBId == currentUser.Id && f.UserAId == userId)));

            if (!isFriend && userId != currentUser.Id) return Forbid("Access denied.");

            var posts = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Recipient)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .Where(p => p.AuthorId == userId || p.RecipientId == userId)
                .ToListAsync();

            return Ok(posts.Select(p => p.ToPostDto()));
        }

        [HttpGet("{postId}")]
        [Authorize]
        public async Task<IActionResult> GetOnePost(string postId)
        {
            var email = User.GetUserEmail();
            var currentUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (currentUser == null) return Unauthorized("User not found.");

            var post = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Recipient)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p =>
                    p.Id == postId &&
                    (p.AuthorId == currentUser.Id || p.RecipientId == currentUser.Id ||
                     _context.Friendships.Any(f =>
                        f.Status == FriendshipStatus.Accepted &&
                        ((f.UserAId == currentUser.Id && f.UserBId == p.AuthorId) ||
                         (f.UserBId == currentUser.Id && f.UserAId == p.AuthorId)))));

            if (post == null) return NotFound("Post not found.");

            return Ok(post.ToPostDto());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPost([FromBody] CreatePostDto postDto)
        {
            var email = User.GetUserEmail();
            var currentUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (currentUser == null) return Unauthorized("User not found.");

            var newPost = postDto.ToPostFromCreateDto();
            newPost.AuthorId = currentUser.Id;
            newPost.Date = DateTime.UtcNow;

            _context.Posts.Add(newPost);
            await _context.SaveChangesAsync();

            return Ok(newPost.ToPostDto());
        }

        [HttpPost("friend/{friendId}")]
        [Authorize]
        public async Task<IActionResult> PostToFriend(string friendId, [FromBody] CreatePostDto postDto)
        {
            var email = User.GetUserEmail();
            var currentUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (currentUser == null) return Unauthorized("User not found.");

            var isFriend = await _context.Friendships.AnyAsync(f =>
                f.Status == FriendshipStatus.Accepted &&
                ((f.UserAId == currentUser.Id && f.UserBId == friendId) ||
                 (f.UserBId == currentUser.Id && f.UserAId == friendId)));

            if (!isFriend) return Forbid("Recipient must be a friend.");

            var newPost = postDto.ToPostFromCreateDto();
            newPost.AuthorId = currentUser.Id;
            newPost.RecipientId = friendId;
            newPost.Date = DateTime.UtcNow;

            _context.Posts.Add(newPost);
            await _context.SaveChangesAsync();

            return Ok(newPost.ToPostDto());
        }

        [HttpDelete("{postId}")]
        [Authorize]
        public async Task<IActionResult> DeletePost(string postId)
        {
            var email = User.GetUserEmail();
            var currentUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (currentUser == null) return Unauthorized("User not found.");

            var post = await _context.Posts.FirstOrDefaultAsync(p =>
                p.Id == postId &&
                (p.AuthorId == currentUser.Id || p.RecipientId == currentUser.Id));

            if (post == null) return NotFound("Post not found or insufficient permissions.");

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return Ok("Post deleted successfully.");
        }
    }
}
