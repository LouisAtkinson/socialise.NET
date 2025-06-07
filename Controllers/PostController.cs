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
        private readonly INotificationRepository _notificationService;

        public PostController(UserManager<User> userManager, ApplicationDbContext context, INotificationRepository notificationService)
        {
            _userManager = userManager;
            _context = context;
            _notificationService = notificationService;
        }

        [HttpGet("all")]
        [Authorize]
        public async Task<IActionResult> GetAllPosts()
        {
            var currentUserId = User.GetUserId();
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());

            if (currentUser == null) return Unauthorized(new { error = "User not found." });

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
                .OrderByDescending(p => p.Date)
                .ToListAsync();

            return Ok(posts.Select(p => p.ToPostDto()));
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUserPosts(string userId)
        {
            var currentUserId = User.GetUserId();
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());

            if (currentUser == null) return Unauthorized(new { error = "User not found." });

            var isFriend = await _context.Friendships.AnyAsync(f =>
                f.Status == FriendshipStatus.Accepted &&
                ((f.UserAId == currentUser.Id && f.UserBId == userId) ||
                 (f.UserBId == currentUser.Id && f.UserAId == userId)));

            if (!isFriend && userId != currentUser.Id) return StatusCode(403, new { error = "Access denied." });

            var posts = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Recipient)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .Where(p => p.AuthorId == userId || p.RecipientId == userId)
                .OrderByDescending(p => p.Date)
                .ToListAsync();

            return Ok(posts.Select(p => p.ToPostDto()));
        }

        [HttpGet("{postId}")]
        [Authorize]
        public async Task<IActionResult> GetOnePost(int postId)
        {
            var currentUserId = User.GetUserId();
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());

            if (currentUser == null) return Unauthorized(new { error = "User not found." });

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

            if (post == null) return NotFound(new { error = "Post not found." });

            return Ok(post.ToPostDto());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPost([FromBody] CreatePostDto postDto)
        {
            var currentUserId = User.GetUserId();
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());

            if (currentUser == null) return Unauthorized(new { error = "User not found." });

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
            var currentUserId = User.GetUserId();
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());

            if (currentUser == null) return Unauthorized(new { error = "User not found." });

            var isFriend = await _context.Friendships.AnyAsync(f =>
                f.Status == FriendshipStatus.Accepted &&
                ((f.UserAId == currentUser.Id && f.UserBId == friendId) ||
                 (f.UserBId == currentUser.Id && f.UserAId == friendId)));

            if (!isFriend) return StatusCode(403, new { error = "Recipient must be a friend." });

            var newPost = postDto.ToPostFromCreateDto();
            newPost.AuthorId = currentUser.Id;
            newPost.RecipientId = friendId;
            newPost.Date = DateTime.UtcNow;

            _context.Posts.Add(newPost);
            await _context.SaveChangesAsync();

            var notification = new Notification
            {
                SenderId = currentUser.Id,
                RecipientId = friendId,
                Type = NotificationType.postFromFriend,
                Timestamp = DateTime.UtcNow,
                PostId = newPost.Id
            };

            await _notificationService.AddAsync(notification);

            return Ok(newPost.ToPostDto());
        }

        [HttpDelete("{postId}")]
        [Authorize]
        public async Task<IActionResult> DeletePost(int postId)
        {
            var currentUserId = User.GetUserId();
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());

            if (currentUser == null) return Unauthorized(new { error = "User not found." });

            var post = await _context.Posts.FirstOrDefaultAsync(p =>
                p.Id == postId &&
                (p.AuthorId == currentUser.Id || p.RecipientId == currentUser.Id));

            if (post == null) return NotFound(new { error = "Post not found or insufficient permissions." });

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Post deleted successfully." });
        }

        [HttpPost("{id}/like")]
        [Authorize]
        public async Task<IActionResult> LikePost(int id)
        {
            var currentUserId = User.GetUserId();
            var currentUser = await _userManager.FindByIdAsync(currentUserId);

            if (currentUser == null) return Unauthorized(new { error = "User not found." });

            var post = await _context.Posts.Include(p => p.Likes).FirstOrDefaultAsync(p => p.Id == id);
            if (post == null) return NotFound(new { error = "Post not found." });

            if (!post.Likes.Contains(currentUser))
            {
                post.Likes.Add(currentUser);
                await _context.SaveChangesAsync();

                if (currentUser.Id != post.AuthorId)
                {
                    var notification = new Notification
                    {
                        SenderId = currentUser.Id,
                        RecipientId = post.AuthorId,
                        Type = NotificationType.postLike,
                        Timestamp = DateTime.UtcNow,
                        PostId = post.Id
                    };

                    await _notificationService.AddAsync(notification);
                }
            }

            return Ok(new { message = "Post liked." });
        }

        [HttpDelete("{id}/unlike")]
        [Authorize]
        public async Task<IActionResult> UnlikePost(int id)
        {
            var currentUserId = User.GetUserId();
            var currentUser = await _userManager.FindByIdAsync(currentUserId);

            if (currentUser == null) return Unauthorized(new { error = "User not found." });

            var post = await _context.Posts.Include(p => p.Likes).FirstOrDefaultAsync(p => p.Id == id);
            if (post == null) return NotFound(new { error = "Post not found." });

            if (post.Likes.Contains(currentUser))
            {
                post.Likes.Remove(currentUser);
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Post unliked." });
        }
    }
}
