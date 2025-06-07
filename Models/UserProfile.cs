using Microsoft.AspNetCore.Identity;

namespace api.Models
{
    public class UserProfile
    {
        public string UserId { get; set; } = string.Empty;
        public VisibilitySettings Visibility { get; set; } = new VisibilitySettings();

        public string BirthDay { get; set; } = string.Empty;
        public string BirthMonth { get; set; } = string.Empty;
        public string Hometown { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
    }

    public class VisibilitySettings
    {
        public bool Birthday { get; set; } = false;

        public bool Hometown { get; set; } = false;

        public bool Occupation { get; set; } = false;
    }
}