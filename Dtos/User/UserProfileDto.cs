using api.Models;

namespace api.Dtos
{
    public class UserProfileDto
    {
        public VisibilitySettingsDto Visibility { get; set; } = new VisibilitySettingsDto();
        public string BirthDay { get; set; } = string.Empty;
        public string BirthMonth { get; set; } = string.Empty;
        public string Hometown { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
    }

    public class VisibilitySettingsDto
    {
        public bool Birthday { get; set; }
        public bool Hometown { get; set; }
        public bool Occupation { get; set; }
    }
}