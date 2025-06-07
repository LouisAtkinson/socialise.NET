using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class DisplayPicture
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public byte[] ImageData { get; set; }
    public byte[] ThumbnailData { get; set; }
    public DateTime UploadDate { get; set; }
    public virtual User User { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<User> Likes { get; set; } = new List<User>();
}
}