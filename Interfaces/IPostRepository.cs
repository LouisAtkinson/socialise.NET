using api.Models;

namespace api.Interfaces
{
	public interface IPostRepository
	{
		Task<Post?> GetByIdAsync(int id);

		Task<Post?> DeleteAsync(int id);

		Task<bool> PostExists(int id);
	}
}