using api.Models;
using api.Helpers;

namespace api.Interfaces
{
	public interface IPostRepository
	{
        Task<List<Post>> GetAllAsync(QueryObject query);

        Task<Post?> GetByIdAsync(string id);

		Task<Post?> DeleteAsync(string id);

		Task<bool> PostExists(string id);
	}
}