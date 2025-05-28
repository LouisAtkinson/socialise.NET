using api.Models;
using api.Helpers;

namespace api.Interfaces
{
	public interface IPostRepository
	{
        Task<List<Post>> GetAllAsync(QueryObject query);

        Task<Post?> GetByIdAsync(int id);

		Task<Post?> DeleteAsync(int id);

		Task<bool> PostExists(int id);
	}
}