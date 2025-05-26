using socialApi.Dtos;
using socialApi.Models;

namespace socialApi.Mappers
{
    public static class DisplayPictureMappers
    {
        public static DisplayPictureDto ToDisplayPictureDto(this DisplayPicture displayPictureModel)
        {
            return new DisplayPictureDto
            {
                Id = displayPictureModel.Id,
                UserId = displayPictureModel.UserId,
                User = displayPictureModel.User,
                Filename = displayPictureModel.Filename,
                UploadDate = displayPictureModel.UploadDate,
                Comments = displayPictureModel.Comments,
                Likes = displayPictureModel.Likes
            };
        }

        public static DisplayPicture ToDisplayPictureModel(this DisplayPictureDto displayPictureDto)
        {
            return new DisplayPicture
            {
                UserId = displayPictureDto.UserId,
                User = displayPictureDto.User,
                Filename = displayPictureDto.Filename,
                UploadDate = displayPictureDto.UploadDate,
                Comments = displayPictureDto.Comments,
                Likes = displayPictureDto.Likes
            };
        }
    }
}