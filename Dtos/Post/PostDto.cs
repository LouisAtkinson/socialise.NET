using api.Models;

namespace api.Dtos
{
    public class PostDto
    {
        public int Id { get; set; }  
        
        public string AuthorId { get; set; }
        public UserSummaryDto? Author { get; set; } 

        public string? RecipientId { get; set; }
        public UserSummaryDto? Recipient { get; set; }

        public string Content { get; set; } = string.Empty;

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public List<UserMinimalDto> Likes { get; set; } = new List<UserMinimalDto>();

        public ICollection<CommentDto> Comments { get; set; } = new List<CommentDto>();
    }
}