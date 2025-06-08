using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using api.Data;
using api.Dtos;
using api.Models;
using api.Mappers;
using api.Repositories;
using api.Interfaces;
using api.Helpers;
using api.Extensions;

namespace api.Controllers
{
    [Route("api/comments")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signinManager;
        private readonly INotificationRepository _notificationService;
        private readonly ICommentRepository _commentRepo;
        private readonly IPostRepository _postRepo;

        public CommentController(UserManager<User> userManager, ITokenService tokenService, SignInManager<User> signInManager, ApplicationDbContext context, ICommentRepository commentRepo, IPostRepository postRepo, INotificationRepository notificationService)
        {
            _commentRepo = commentRepo;
            _context = context;
            _postRepo = postRepo;
            _userManager = userManager;
            _tokenService = tokenService;
            _signinManager = signInManager;
            _notificationService = notificationService;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var comment = await _commentRepo.GetByIdAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment.ToCommentDto());
        }

        [HttpPost("post/{postId}")]
        [Authorize]
        public async Task<IActionResult> AddCommentToPost(int postId, [FromBody] CreateCommentDto commentDto)
        {
            var currentUserId = User.GetUserId();
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());

            if (currentUser == null) return Unauthorized(new { error = "User not found." });

            var post = await _context.Posts.FirstOrDefaultAsync(p =>
                p.Id == postId &&
                (p.AuthorId == currentUser.Id ||
                p.RecipientId == currentUser.Id ||
                _context.Friendships.Any(f =>
                    f.Status == FriendshipStatus.Accepted &&
                    ((f.UserAId == currentUser.Id && f.UserBId == p.AuthorId) ||
                    (f.UserBId == currentUser.Id && f.UserAId == p.AuthorId)))
                ));

            if (post == null) return NotFound(new { error = "Post not accessible or does not exist." });

            var comment = new Comment
            {
                AuthorId = currentUser.Id,
                PostId = postId,
                Content = commentDto.Content,
                Date = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok(comment.ToCommentDto());
        }

        [HttpPost("display-picture/{displayPictureId}")]
        [Authorize]
        public async Task<IActionResult> AddCommentToDisplayPicture(int displayPictureId, [FromBody] CreateCommentDto commentDto)
        {
            var currentUserId = User.GetUserId();
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());

            if (currentUser == null)
                return Unauthorized(new { error = "User not found." });

            var dp = await _context.DisplayPictures.FirstOrDefaultAsync(p =>
                p.Id == displayPictureId &&
                (p.UserId == currentUser.Id ||
                _context.Friendships.Any(f =>
                    f.Status == FriendshipStatus.Accepted &&
                    ((f.UserAId == currentUser.Id && f.UserBId == p.UserId) ||
                    (f.UserBId == currentUser.Id && f.UserAId == p.UserId)))
                ));

            if (dp == null)
                return NotFound(new { error = "Display picture not accessible or does not exist." });

            var comment = new Comment
            {
                AuthorId = currentUser.Id,
                DisplayPictureId = displayPictureId, 
                Content = commentDto.Content,
                Date = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok(comment.ToCommentDto());
        }

        [HttpPost("{commentId}/like")]
        [Authorize]
        public async Task<IActionResult> LikeComment(int commentId)
        {
            var currentUserId = User.GetUserId();
            var currentUser = await _userManager.FindByIdAsync(currentUserId);

            if (currentUser == null) 
                return Unauthorized(new { error = "User not found." });

            var comment = await _context.Comments
                .Include(c => c.Likes)
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null) 
                return NotFound(new { error = "Comment not found." });

            if (!comment.Likes.Contains(currentUser))
            {
                comment.Likes.Add(currentUser);
                await _context.SaveChangesAsync();

                if (currentUser.Id != comment.AuthorId)
                {
                    var notification = new Notification
                    {
                        SenderId = currentUser.Id,
                        RecipientId = comment.AuthorId,
                        Type = NotificationType.commentLike,
                        Timestamp = DateTime.UtcNow,
                        CommentId = comment.Id
                    };

                    await _notificationService.AddAsync(notification);
                }

            }

            return Ok(new { message = "Comment liked." });
        }

        [HttpDelete("{commentId}/unlike")]
        [Authorize]
        public async Task<IActionResult> UnlikeComment(int commentId)
        {
            var currentUserId = User.GetUserId();
            var currentUser = await _userManager.FindByIdAsync(currentUserId);

            if (currentUser == null) 
                return Unauthorized(new { error = "User not found." });

            var comment = await _context.Comments
                .Include(c => c.Likes)
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null) 
                return NotFound(new { error = "Comment not found." });

            if (comment.Likes.Contains(currentUser))
            {
                comment.Likes.Remove(currentUser);
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Comment unliked." });
        }

        [HttpDelete("{commentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var currentUserId = User.GetUserId();
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());

            if (currentUser == null) return Unauthorized(new { error = "User not found." });

            var comment = await _context.Comments.FirstOrDefaultAsync(c =>
                c.Id == commentId && c.AuthorId == currentUser.Id);

            if (comment == null) return NotFound(new { error = "Comment not found or insufficient permissions." });

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Comment deleted successfully." });
        }
    }
}