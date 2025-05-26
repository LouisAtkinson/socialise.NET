using socialApi.Dtos;
using socialApi.Models;

namespace socialApi.Mappers
{
    public static class PostMappers
    {
        public static PostDto ToPostDto(this Post postModel)
        {
            return new PostDto
            {
                Id = postModel.Id,
                AuthorId = postModel.AuthorId,
                Author = postModel.Author,
                RecipientId = postModel.RecipientId,
                Recipient = postModel.Recipient,
                Content = postModel.Content,
                Date = postModel.Date,
                Likes = postModel.Likes,
                Comments = postModel.Comments
            };
        }
        
        public static Post ToPostFromDto(this PostDto postDto)
        {
            return new Post
            {
                AuthorId = postDto.AuthorId,
                Author = postDto.Author,
                RecipientId = postDto.RecipientId,
                Recipient = postDto.Recipient,
                Content = postDto.Content,
                Date = postDto.Date,
                Likes = postDto.Likes,
                Comments = postDto.Comments
            };
        }
    }
    
    
}