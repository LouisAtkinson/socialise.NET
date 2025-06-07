namespace api.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public string AuthorId { get; set; }
        public User? Author { get; set; }

        public string Content { get; set; } = string.Empty;

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public int? PostId { get; set; }
        public int? DisplayPictureId { get; set; }
        public Post? Post { get; set; }
        public ICollection<User> Likes { get; set; } = new List<User>();
    }
}