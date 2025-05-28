using Microsoft.AspNetCore.Identity;

namespace api.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public VisibilitySettings Visibility { get; set; } = new VisibilitySettings();

        public string BirthDay { get; set; } = string.Empty;

        public string BirthMonth { get; set; } = string.Empty;

        public string Hometown { get; set; } = string.Empty;

        public string Occupation { get; set; } = string.Empty;

        public int? DisplayPictureId { get; set; }
        public DisplayPicture DisplayPicture { get; set; }

        public ICollection<Post> Posts { get; set; } = new List<Post>();

        public ICollection<User> Friends { get; set; } = new List<User>();

        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }

    public class VisibilitySettings
    {
        public bool Birthday { get; set; } = false;

        public bool Hometown { get; set; } = false;

        public bool Occupation { get; set; } = false;
    }
}