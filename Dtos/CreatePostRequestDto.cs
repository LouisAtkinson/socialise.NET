using socialApi.Models;

namespace socialApi.Dtos
{
    public class CreatedPostRequestDto
    {
        
        public int AuthorId { get; set; }
        public User? Author { get; set; } 

        public int? RecipientId { get; set; }
        public User? Recipient { get; set; }

        public string Content { get; set; } = string.Empty;

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public ICollection<User> Likes { get; set; } = new List<User>();

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}