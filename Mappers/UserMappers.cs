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
            };
        }

            public static NewUserDto ToNewUserDto(this User user, string token)
        {
            return new NewUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Token = token
            };
        }

        public static User ToUserFromDto(this UserDto userDto)
        {
            return new User
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                UserName = userDto.Email
            };
        }

        public static User ToUserFromDto(this RegisterDto registerDto)
        {
            return new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email.ToLower(),
                UserName = registerDto.Email.ToLower()
            };
        }

        public static UserSummaryDto ToUserSummaryDto(this User user)
        {
            return new UserSummaryDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DisplayPictureId = user.DisplayPicture?.Id
            };
        }

        public static UserMinimalDto ToUserMinimalDto(this User user)
        {
            return new UserMinimalDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }
    }
}