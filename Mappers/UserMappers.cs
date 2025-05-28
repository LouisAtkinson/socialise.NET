using api.Dtos;
using api.Models;

namespace api.Mappers
{
    public static class UserMappers
    {
        public static UserDto ToUserDto(this User userModel)
    {
        return new UserDto
        {
            Id = userModel.Id,
            FirstName = userModel.FirstName,
            LastName = userModel.LastName,
            Email = userModel.Email,
            Visibility = new VisibilitySettingsDto
            {
                Birthday = userModel.Visibility.Birthday,
                Hometown = userModel.Visibility.Hometown,
                Occupation = userModel.Visibility.Occupation
            },
            BirthDay = userModel.BirthDay,
            BirthMonth = userModel.BirthMonth,
            Hometown = userModel.Hometown,
            Occupation = userModel.Occupation,
            DisplayPictureId = userModel.DisplayPictureId,
            DisplayPicture = userModel.DisplayPicture,
            Posts = userModel.Posts,
            Friends = userModel.Friends,
            Notifications = userModel.Notifications
        };
    }

    public static User ToUserFromDto(this UserDto userDto)
        {
            return new User
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                Visibility = new VisibilitySettings
                {
                    Birthday = userDto.Visibility.Birthday,
                    Hometown = userDto.Visibility.Hometown,
                    Occupation = userDto.Visibility.Occupation
                },
                BirthDay = userDto.BirthDay,
                BirthMonth = userDto.BirthMonth,
                Hometown = userDto.Hometown,
                Occupation = userDto.Occupation,
                DisplayPictureId = userDto.DisplayPictureId,
                DisplayPicture = userDto.DisplayPicture,
                Posts = userDto.Posts,
                Friends = userDto.Friends,
                Notifications = userDto.Notifications
            };
        }
    }
}