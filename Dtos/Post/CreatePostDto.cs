using api.Models;

namespace api.Dtos
{
    public class CreatePostDto
    {
        public int? RecipientId { get; set; }

        public string Content { get; set; } = string.Empty;
    }
}