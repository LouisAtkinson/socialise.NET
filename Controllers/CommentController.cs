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

        private readonly ICommentRepository _commentRepo;
        private readonly IPostRepository _postRepo;

        public CommentController(UserManager<User> userManager, ITokenService tokenService, SignInManager<User> signInManager, ApplicationDbContext context, ICommentRepository commentRepo, IPostRepository postRepo)
        {
            _commentRepo = commentRepo;
            _context = context;
            _postRepo = postRepo;
            _userManager = userManager;
            _tokenService = tokenService;
            _signinManager = signInManager;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id) {
            var comment = await _commentRepo.GetByIdAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment.ToCommentDto());
        }

        [HttpPost("{postId}/comments")]
        [Authorize]
        public async Task<IActionResult> AddComment(int postId, [FromBody] CreateCommentDto commentDto)
        {
            var currentUserId = User.GetUserId();
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());

            if (currentUser == null) return Unauthorized("User not found.");

            var post = await _context.Posts.FirstOrDefaultAsync(p =>
                p.Id == postId &&
                (p.AuthorId == currentUser.Id ||
                p.RecipientId == currentUser.Id ||
                _context.Friendships.Any(f =>
                    f.Status == FriendshipStatus.Accepted &&
                    ((f.UserAId == currentUser.Id && f.UserBId == p.AuthorId) ||
                    (f.UserBId == currentUser.Id && f.UserAId == p.AuthorId)))
                ));

            if (post == null) return NotFound("Post not accessible or does not exist.");

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

        [HttpDelete("comments/{commentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var currentUserId = User.GetUserId();
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());

            if (currentUser == null) return Unauthorized("User not found.");

            var comment = await _context.Comments.FirstOrDefaultAsync(c =>
                c.Id == commentId && c.AuthorId == currentUser.Id);

            if (comment == null) return NotFound("Comment not found or insufficient permissions.");

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok("Comment deleted successfully.");
        }
    }
}