using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Dtos;
using api.Models;
using api.Mappers;
using api.Repositories;
using api.Interfaces;

namespace api.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly IPostRepository _postRepo;

        public PostController(ApplicationDbContext context, IPostRepository postRepo)
        {
            _postRepo = postRepo;
            _context = context;
        }

        [HttpGet]
        // public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        // {
        //     if (!ModelState.IsValid)
        //         return BadRequest(ModelState);
        //
        //     var posts = await _postRepo.GetAllAsync(query);
        //
        //     var postDto = posts.Select(p => p.ToPostDto()).ToList();
        //
        //     return Ok(postDto);
        // }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id) {
            var post = await _postRepo.GetByIdAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return Ok(post.ToPostDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePostDto postDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var postModel = postDto.ToPostFromCreateDto();
            await _context.Posts.AddAsync(postModel);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = postModel.Id }, postModel.ToPostDto());
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var postModel = await _postRepo.DeleteAsync(id);

            if (postModel == null)
            {
                return NotFound("Post does not exist");
            }

            return Ok(postModel);
        }
    }
}