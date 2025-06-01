namespace api.Models
{
    public class Comment
    {
        public string Id { get; set; }

        public string AuthorId { get; set; }
        public User? Author { get; set; }

        public string Content { get; set; } = string.Empty;

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public string PostId { get; set; }
        public Post? Post { get; set; }
    }
}