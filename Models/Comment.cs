namespace socialApi.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public int AuthorId { get; set; }
        public User? Author { get; set; }

        public string Content { get; set; } = string.Empty;

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public int PostId { get; set; }
        public Post? Post { get; set; }

        public int? DisplayPictureId { get; set; }
        public DisplayPicture? DisplayPicture { get; set; }
    }
}