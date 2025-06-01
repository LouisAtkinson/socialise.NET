using api.Models;

namespace api.Dtos
{
    public class DisplayPictureDto
    {
        public string Id { get; set; } 

        public string UserId { get; set; }
        public User? User { get; set; }

        public string Filename { get; set; } = string.Empty;

        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<User> Likes { get; set; } = new List<User>();
    }
}