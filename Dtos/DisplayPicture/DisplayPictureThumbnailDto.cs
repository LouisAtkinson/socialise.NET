using api.Models;

namespace api.Dtos
{
    public class DisplayPictureThumbnailDto
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public byte[] ThumbnailData { get; set; } 
}
}