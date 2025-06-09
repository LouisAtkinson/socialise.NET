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
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signinManager;

        public UserController(UserManager<User> userManager, ITokenService tokenService, SignInManager<User> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signinManager = signInManager;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == loginDto.Email.ToLower());

            if (user == null)
                return Unauthorized(new { error = "Invalid email or account does not exist." });

            var result = await _signinManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
                return Unauthorized(new { error = "Email not found and/or password incorrect" });

            var token = _tokenService.CreateToken(user);
            var newUserDto = user.ToNewUserDto(token);

            return Ok(newUserDto);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = registerDto.ToUserFromDto();

                user.Email = user.Email.ToLower();
                user.UserName = user.Email;

                var createdUser = await _userManager.CreateAsync(user, registerDto.Password);

                if (!createdUser.Succeeded)
                {
                    return StatusCode(500, new
                    {
                        error = string.Join(" ", createdUser.Errors.Select(e => e.Description))
                    });
                }

                var roleResult = await _userManager.AddToRoleAsync(user, "User");

                if (!roleResult.Succeeded)
                {
                    return StatusCode(500, new
                    {
                        error = string.Join(" ", roleResult.Errors.Select(e => e.Description))
                    });
                }

                var userProfile = new UserProfile
                {
                    UserId = user.Id,
                };

                _context.UserProfiles.Add(userProfile);
                await _context.SaveChangesAsync();

                user.UserProfile = userProfile;

                var token = _tokenService.CreateToken(user);

                var newUserDto = user.ToNewUserDto(token);

                return Ok(newUserDto);
            }
            catch (Exception e)
            {
                return StatusCode(500, new
                {
                    error = new List<string> { "An unexpected error occurred.", e.Message }
                });
            }
        }


        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById([FromRoute] string id)
        {
            var user = await _context.Users
                .Include(u => u.DisplayPicture)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null) return NotFound();

            var userProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(up => up.UserId == id);

            var dto = user.ToUserFullProfileDto(userProfile);

            return Ok(dto);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUserProfile(string id, [FromBody] UserProfileDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUserId = User.GetUserId();

            if (id != currentUserId)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "You are not authorised to update this profile." });
            }

            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == id);

            if (userProfile == null)
            {
                return NotFound(new { error = "User profile not found." });
            }

            try
            {
                userProfile.BirthDay = dto.BirthDay;
                userProfile.BirthMonth = dto.BirthMonth;
                userProfile.Hometown = dto.Hometown;
                userProfile.Occupation = dto.Occupation;

                userProfile.Visibility.Birthday = dto.Visibility.Birthday;
                userProfile.Visibility.Hometown = dto.Visibility.Hometown;
                userProfile.Visibility.Occupation = dto.Visibility.Occupation;

                _context.UserProfiles.Update(userProfile);
                await _context.SaveChangesAsync();

                return Ok(new { message = "User profile updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpGet("search/{query}")]
        [Authorize] 
        public async Task<IActionResult> SearchUsers(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new { error = "Query parameter is required." });

            var loweredQuery = query.ToLower();

            var users = await _context.Users
                .Where(u => u.FirstName.ToLower().StartsWith(loweredQuery) ||
                            u.LastName.ToLower().StartsWith(loweredQuery))
                .Include(u => u.DisplayPicture)
                .ToListAsync();

            var userSummaries = users.Select(u => u.ToUserSummaryDto()).ToList();

            return Ok(userSummaries);
        }
    }
}