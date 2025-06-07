using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using api.Data;
using api.Dtos;
using api.Models;
using api.Mappers;
using api.Interfaces;

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

            if (user == null) return Unauthorized(new { error = "Invalid email or account does not exist." });

            var result = await _signinManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded) return Unauthorized(new { error = "Email not found and/or password incorrect" });

            return Ok(
                new NewUserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Token = _tokenService.CreateToken(user)
                }
            );
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = new User
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Email = registerDto.Email.ToLower(),
                    UserName = registerDto.Email.ToLower()
                };

                var createdUser = await _userManager.CreateAsync(user, registerDto.Password);

                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, "User");
                    if (roleResult.Succeeded)
                    {
                        return Ok(
                            new NewUserDto
                            {
                                Id = user.Id,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Email = user.Email,
                                Token = _tokenService.CreateToken(user)
                            }
                        );
                    }
                    else
                    {
                        return StatusCode(500, new
                        {
                            error = string.Join(" ", createdUser.Errors.Select(e => e.Description))
                        });
                    }
                }
                else
                {
                    return StatusCode(500, new
                    {
                        error = string.Join(" ", createdUser.Errors.Select(e => e.Description))
                    });
                }
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
    }
}