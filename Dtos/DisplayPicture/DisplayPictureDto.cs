using api.Models;

namespace api.Dtos
{
    public class DisplayPictureDto
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public UserMinimalDto? User { get; set; }
    public DateTime UploadDate { get; set; }
    
    public byte[]? ImageData { get; set; }

    public List<CommentDto> Comments { get; set; } = new List<CommentDto>();
    public List<UserMinimalDto> Likes { get; set; } = new List<UserMinimalDto>();
}
}