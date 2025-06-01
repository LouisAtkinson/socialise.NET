using Microsoft.AspNetCore.Identity;

namespace api.Models
{
    public class User : IdentityUser
    {
        public required string FirstName { get; set; } = string.Empty;

        public required string LastName { get; set; } = string.Empty;
    }
}