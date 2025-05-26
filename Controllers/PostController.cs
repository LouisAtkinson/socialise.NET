using Microsoft.AspNetCore.Mvc;
using socialApi.Data;
using socialApi.Dtos;
using socialApi.Models;
using socialApi.Mappers;

namespace socialApi.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id) {
            var post = _context.Post.Find(id);

            if (post == null)
            {
                return NotFound();
            }

            return Ok(post.ToPostDto());
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreatePostRequestDto postDto)
        {
            var postModel = postDto.ToPostFromDto();
            _context.Post.Add(postModel);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = postModel.Id }, postModel.ToPostDto());
        }
    }
}