using api.Models;

namespace api.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }
        
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }
}