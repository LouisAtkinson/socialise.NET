using api.Models;

namespace api.Dtos
{
    public class CommentDto
    {
        public int Id { get; set; }

        public string AuthorId { get; set; }
        public UserSummaryDto? Author { get; set; }

        public string Content { get; set; } = string.Empty;

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public int PostId { get; set; }
        public Post? Post { get; set; }
    }
}