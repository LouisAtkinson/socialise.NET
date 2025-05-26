using Microsoft.AspNetCore.Mvc;
using socialApi.Data;
using socialApi.Dtos;
using socialApi.Models;
using socialApi.Mappers;

namespace socialApi.Controllers
{
    [Route("api/comments")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id) {
            var comment = _context.Comment.Find(id);

            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment.ToCommentDto());
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateCommentRequestDto commentDto)
        {
            var commentModel = commentDto.ToCommentFromDto();
            _context.Comment.Add(commentModel);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = commentModel.Id }, commentModel.ToSCommentDto())
        }
    }
}