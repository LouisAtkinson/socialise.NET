using api.Models;

namespace api.Interfaces
{
    public interface ICommentRepository
    {
        Task<Comment?> GetByIdAsync(string id);

        Task<Comment> CreateAsync(Comment commentModel);

        Task<Comment?> DeleteAsync(string id);
    }
}