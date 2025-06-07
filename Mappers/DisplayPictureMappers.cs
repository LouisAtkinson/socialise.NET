using api.Dtos;
using api.Models;

namespace api.Mappers
{
    public static class DisplayPictureMappers
    {
        public static DisplayPictureDto ToDisplayPictureDto(this DisplayPicture displayPictureModel)
        {
            return new DisplayPictureDto
            {
                Id = displayPictureModel.Id,
                UserId = displayPictureModel.UserId,
                User = displayPictureModel.User?.ToUserMinimalDto(),
                UploadDate = displayPictureModel.UploadDate,
                Comments = displayPictureModel.Comments.Select(c => c.ToCommentDto()).ToList(),
                Likes = displayPictureModel.Likes.Select(u => u.ToUserMinimalDto()).ToList()
            };
        }

        public static DisplayPicture ToDisplayPictureModel(this DisplayPictureDto displayPictureDto)
        {
            return new DisplayPicture
            {
                Id = displayPictureDto.Id,
                UserId = displayPictureDto.UserId,
                UploadDate = displayPictureDto.UploadDate,
            };
        }
    }
}