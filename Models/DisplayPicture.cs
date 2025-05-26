namespace socialApi.Models
{
    public class DisplayPicture
    {
        public int Id { get; set; } 

        public int UserId { get; set; }
        public User User { get; set; }

        public string Filename { get; set; } = string.Empty;

        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<User> Likes { get; set; } = new List<User>();
    }
}