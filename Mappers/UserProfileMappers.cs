using api.Dtos;
using api.Models;

namespace api.Mappers
{
    public static class UserProfileMappers
    {
        public static UserProfileDto ToUserProfileDto(this UserProfile userProfileModel)
        {
            return new UserProfileDto
            {
                BirthDay = userProfileModel.BirthDay,
                BirthMonth = userProfileModel.BirthMonth,
                Hometown = userProfileModel.Hometown,
                Occupation = userProfileModel.Occupation,
                Visibility = new VisibilitySettingsDto
                {
                    Birthday = userProfileModel.Visibility.Birthday,
                    Hometown = userProfileModel.Visibility.Hometown,
                    Occupation = userProfileModel.Visibility.Occupation,
                }
            };
        }

        public static UserProfile ToUserProfileFromDto(this UserProfileDto userProfileDto)
        {
            return new UserProfile
            {
                BirthDay = userProfileDto.BirthDay,
                BirthMonth = userProfileDto.BirthMonth,
                Hometown = userProfileDto.Hometown,
                Occupation = userProfileDto.Occupation,
                Visibility = new VisibilitySettings
                {
                    Birthday = userProfileDto.Visibility.Birthday,
                    Hometown = userProfileDto.Visibility.Hometown,
                    Occupation = userProfileDto.Visibility.Occupation,
                }
            };
        }
    }
}
