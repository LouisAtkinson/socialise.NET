using api.Dtos;
using api.Models;

namespace api.Mappers
{
    public static class PostMappers
    {
        public static PostDto ToPostDto(this Post postModel)
        {
            return new PostDto
            {
                Id = postModel.Id,
                AuthorId = postModel.AuthorId,
                Author = postModel.Author?.ToUserSummaryDto(),
                RecipientId = postModel.RecipientId,
                Recipient = postModel.Recipient?.ToUserSummaryDto(),
                Content = postModel.Content,
                Date = postModel.Date,
                Likes = postModel.Likes.Select(u => u.ToUserMinimalDto()).ToList(),
                Comments = postModel.Comments.Select(c => c.ToCommentDto()).ToList()
            };
        }
        
        public static Post ToPostFromCreateDto(this CreatePostDto postDto)
        {
            return new Post
            {
                RecipientId = postDto.RecipientId,
                Content = postDto.Content
            };
        }
    }
    
    
}