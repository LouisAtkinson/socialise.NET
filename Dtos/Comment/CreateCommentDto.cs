using api.Models;

namespace api.Dtos
{
    public class CreateCommentDto
    {
        public int PostId { get; set; }  

        public string Content { get; set; } = string.Empty;
    }
}