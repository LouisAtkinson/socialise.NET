using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using api.Data;
using api.Dtos;
using api.Models;
using api.Mappers;
using api.Extensions;

namespace api.Controllers
{
    [Route("api/display-pictures")]
    [ApiController]
    public class DisplayPictureController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public DisplayPictureController(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> UploadDisplayPicture([FromBody] DisplayPictureDto displayPictureDto)
        {
            var email = User.GetUserEmail();
            var currentUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (currentUser == null) return Unauthorized("User not found.");

            var newDisplayPicture = displayPictureDto.ToDisplayPictureModel();
            newDisplayPicture.UserId = currentUser.Id;

            _context.DisplayPictures.Add(newDisplayPicture);
            await _context.SaveChangesAsync();

            return Ok(newDisplayPicture.ToDisplayPictureDto());
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetDisplayPictureByUserId(string userId)
        {
            var displayPicture = await _context.DisplayPictures
                .Include(dp => dp.User)
                .Include(dp => dp.Comments)
                .Include(dp => dp.Likes)
                .FirstOrDefaultAsync(dp => dp.UserId == userId);

            if (displayPicture == null) return NotFound("Display picture not found.");

            return Ok(displayPicture.ToDisplayPictureDto());
        }

        [HttpGet("{id}/details")]
        [Authorize]
        public async Task<IActionResult> GetDisplayPictureDetails(string id)
        {
            var displayPicture = await _context.DisplayPictures
                .Include(dp => dp.User)
                .Include(dp => dp.Comments)
                    .ThenInclude(c => c.Author)
                .Include(dp => dp.Likes)
                .FirstOrDefaultAsync(dp => dp.Id == id);

            if (displayPicture == null) return NotFound("Display picture not found.");

            return Ok(displayPicture.ToDisplayPictureDto());
        }

        [HttpPost("{id}/like")]
        [Authorize]
        public async Task<IActionResult> LikeDisplayPicture(string id)
        {
            var email = User.GetUserEmail();
            var currentUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (currentUser == null) return Unauthorized("User not found.");

            var displayPicture = await _context.DisplayPictures.Include(dp => dp.Likes).FirstOrDefaultAsync(dp => dp.Id == id);

            if (displayPicture == null) return NotFound("Display picture not found.");

            if (!displayPicture.Likes.Contains(currentUser))
            {
                displayPicture.Likes.Add(currentUser);
                await _context.SaveChangesAsync();
            }

            return Ok("Display picture liked.");
        }

        [HttpPost("{id}/unlike")]
        [Authorize]
        public async Task<IActionResult> UnlikeDisplayPicture(string id)
        {
            var email = User.GetUserEmail();
            var currentUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (currentUser == null) return Unauthorized("User not found.");

            var displayPicture = await _context.DisplayPictures.Include(dp => dp.Likes).FirstOrDefaultAsync(dp => dp.Id == id);

            if (displayPicture == null) return NotFound("Display picture not found.");

            if (displayPicture.Likes.Contains(currentUser))
            {
                displayPicture.Likes.Remove(currentUser);
                await _context.SaveChangesAsync();
            }

            return Ok("Display picture unliked.");
        }

        [HttpPost("{id}/comment")]
        [Authorize]
        public async Task<IActionResult> CommentOnDisplayPicture(string id, [FromBody] string commentText)
        {
            var email = User.GetUserEmail();
            var currentUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (currentUser == null) return Unauthorized("User not found.");

            var displayPicture = await _context.DisplayPictures.Include(dp => dp.Comments).FirstOrDefaultAsync(dp => dp.Id == id);

            if (displayPicture == null) return NotFound("Display picture not found.");

            var comment = new Comment
            {
                AuthorId = currentUser.Id,
                Content = commentText,
                Date = DateTime.UtcNow
            };

            displayPicture.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok("Comment added.");
        }

        [HttpDelete("{id}/comment/{commentId}")]
        [Authorize]
        public async Task<IActionResult> RemoveComment(string id, string commentId)
        {
            var email = User.GetUserEmail();
            var currentUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (currentUser == null) return Unauthorized("User not found.");

            var displayPicture = await _context.DisplayPictures.Include(dp => dp.Comments).FirstOrDefaultAsync(dp => dp.Id == id);

            if (displayPicture == null) return NotFound("Display picture not found.");

            var comment = displayPicture.Comments.FirstOrDefault(c => c.Id == commentId && c.AuthorId == currentUser.Id);

            if (comment == null) return NotFound("Comment not found or insufficient permissions.");

            displayPicture.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok("Comment removed.");
        }
    }
}
