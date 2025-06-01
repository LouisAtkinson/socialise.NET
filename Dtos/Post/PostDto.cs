using api.Models;

namespace api.Dtos
{
    public class PostDto
    {
        public string Id { get; set; }  
        
        public string AuthorId { get; set; }
        public User? Author { get; set; } 

        public string? RecipientId { get; set; }
        public User? Recipient { get; set; }

        public string Content { get; set; } = string.Empty;

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public ICollection<UserDto> Likes { get; set; } = new List<UserDto>();

        public ICollection<CommentDto> Comments { get; set; } = new List<CommentDto>();
    }
}