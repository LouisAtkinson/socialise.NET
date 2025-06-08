using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using api.Data;
using api.Dtos;
using api.Models;
using api.Mappers;
using api.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

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
        public async Task<IActionResult> UploadDisplayPicture(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "No file uploaded." });

            var currentUserId = User.GetUserId();
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
            if (currentUser == null) return Unauthorized(new { error = "User not found." });

            try
            {
                using var inputStream = file.OpenReadStream();
                using var image = Image.Load(inputStream);

                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(500, 500)
                }));

                using var outputStream = new MemoryStream();
                image.Save(outputStream, new JpegEncoder { Quality = 70 });
                var compressedImage = outputStream.ToArray();

                if (compressedImage.Length > 100 * 1024)
                    return BadRequest(new { error = "Image could not be compressed below 100KB." });

                // Generate thumbnail
                using var thumbnailImage = image.Clone(ctx => ctx.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Crop,
                    Size = new Size(150, 150)
                }));

                using var thumbnailStream = new MemoryStream();
                thumbnailImage.Save(thumbnailStream, new JpegEncoder { Quality = 50 });
                var thumbnailData = thumbnailStream.ToArray();

                var newDisplayPicture = new DisplayPicture
                {
                    UserId = currentUserId,
                    UploadDate = DateTime.UtcNow,
                    ImageData = compressedImage,
                    ThumbnailData = thumbnailData
                };

                _context.DisplayPictures.Add(newDisplayPicture);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Upload successful.", DisplayPictureId = newDisplayPicture.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpGet("user/{userId}/thumbnail")]
        [Authorize]
        public async Task<IActionResult> GetThumbnailByUserId(string userId)
        {
            var displayPicture = await _context.DisplayPictures
                .AsNoTracking()
                .FirstOrDefaultAsync(dp => dp.UserId == userId);

            if (displayPicture == null)
                return NotFound();

            return File(displayPicture.ThumbnailData, "image/jpeg");
        }

        [HttpGet("user/{userId}/full")]
        [Authorize]
        public async Task<IActionResult> GetFullImageByUserId(string userId)
        {
            var displayPicture = await _context.DisplayPictures
                .AsNoTracking()
                .FirstOrDefaultAsync(dp => dp.UserId == userId);

            if (displayPicture == null)
                return NotFound();

            return File(displayPicture.ImageData, "image/jpeg");
        }

        [HttpGet("user/{userId}/details")]
        [Authorize]
        public async Task<IActionResult> GetDisplayPictureDetailsByUserId(string userId)
        {
            var displayPicture = await _context.DisplayPictures
                .Include(dp => dp.User)
                .Include(dp => dp.Comments)
                    .ThenInclude(c => c.Author)
                .Include(dp => dp.Likes)
                .FirstOrDefaultAsync(dp => dp.User.Id == userId);

            if (displayPicture == null)
                return NotFound();

            var dto = displayPicture.ToDisplayPictureDto();
            return Ok(dto);
        }

        [HttpPost("{id}/like")]
        [Authorize]
        public async Task<IActionResult> LikeDisplayPicture(int id)
        {
            var currentUserId = User.GetUserId();
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());

            if (currentUser == null) return Unauthorized(new { error = "User not found." });

            var displayPicture = await _context.DisplayPictures.Include(dp => dp.Likes).FirstOrDefaultAsync(dp => dp.Id == id);

            if (displayPicture == null) return NotFound(new { error = "Display picture not found." });

            if (!displayPicture.Likes.Contains(currentUser))
            {
                displayPicture.Likes.Add(currentUser);
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Display picture liked." });
        }

        [HttpPost("{id}/unlike")]
        [Authorize]
        public async Task<IActionResult> UnlikeDisplayPicture(int id)
        {
            var currentUserId = User.GetUserId();
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());

            if (currentUser == null) return Unauthorized(new { error = "User not found." });

            var displayPicture = await _context.DisplayPictures.Include(dp => dp.Likes).FirstOrDefaultAsync(dp => dp.Id == id);

            if (displayPicture == null) return NotFound(new { error = "Display picture not found." });

            if (displayPicture.Likes.Contains(currentUser))
            {
                displayPicture.Likes.Remove(currentUser);
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Display picture unliked." });
        }
    }
}
