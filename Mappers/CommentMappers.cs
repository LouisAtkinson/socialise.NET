using api.Dtos;
using api.Models;

namespace api.Mappers
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

        public static Comment ToCommentFromCreateDto(this CreateCommentDto commentDto)
        {
            return new Comment
            {
                Content = commentDto.Content,
                PostId = commentDto.PostId,
            };
        }
    }
}