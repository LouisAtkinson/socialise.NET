using socialApi.Dtos;
using socialApi.Models;

namespace socialApi.Mappers
{
    public static class CommentMappers
    {
        public static CommentDto ToCommentDto(this Comment commentModel)
        {
            return new CommentDto
            {
                Id = commentModel.Id,
                AuthorId = commentModel.AuthorId,
                Author = commentModel.Author,
                Content = commentModel.Content,
                Date = commentModel.Date,
                PostId = commentModel.PostId,
                Post = commentModel.Post,
                DisplayPictureId = commentModel.DisplayPictureId,
                DisplayPicture = commentModel.DisplayPicture
            };
        }

        public static Comment ToCommentModel(this CommentDto commentDto)
        {
            return new Comment
            {
                Id = commentDto.Id,
                AuthorId = commentDto.AuthorId,
                Author = commentDto.Author,
                Content = commentDto.Content,
                Date = commentDto.Date,
                PostId = commentDto.PostId,
                Post = commentDto.Post,
                DisplayPictureId = commentDto.DisplayPictureId,
                DisplayPicture = commentDto.DisplayPicture
            };
        }
    }
}