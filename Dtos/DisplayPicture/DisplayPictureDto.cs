using api.Models;

namespace api.Dtos
{
    public class DisplayPictureDto
    {
        public int Id { get; set; } 

        public string UserId { get; set; }
        public User? User { get; set; }

        public string Filename { get; set; } = string.Empty;

        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public List<UserMinimalDto> Likes { get; set; } = new List<UserMinimalDto>();
    }
}