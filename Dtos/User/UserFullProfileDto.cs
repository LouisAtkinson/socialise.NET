namespace api.Dtos
{
    public class UserFullProfileDto
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int? DisplayPicture { get; set; }

        public string BirthDay { get; set; } = string.Empty;
        public string BirthMonth { get; set; } = string.Empty;
        public string Hometown { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;

        public VisibilitySettingsDto Visibility { get; set; } = new VisibilitySettingsDto();
    }
}