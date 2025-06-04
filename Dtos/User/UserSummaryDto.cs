using api.Models;

namespace api.Dtos
{
    public class UserSummaryDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DisplayPicture? DisplayPicture { get; set; }
    }
}