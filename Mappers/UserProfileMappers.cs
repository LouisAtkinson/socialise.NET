using api.Dtos;
using api.Models;

namespace api.Mappers
{
    public static class UserProfileMappers
    {
            public static UserFullProfileDto ToUserFullProfileDto(this User user, UserProfile? userProfile)
            {
                userProfile ??= new UserProfile();

                return new UserFullProfileDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DisplayPicture = user.DisplayPicture?.Id,

                    BirthDay = userProfile.BirthDay,
                    BirthMonth = userProfile.BirthMonth,
                    Hometown = userProfile.Hometown,
                    Occupation = userProfile.Occupation,
                    Visibility = new VisibilitySettingsDto
                    {
                        Birthday = userProfile.Visibility.Birthday,
                        Hometown = userProfile.Visibility.Hometown,
                        Occupation = userProfile.Visibility.Occupation
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
