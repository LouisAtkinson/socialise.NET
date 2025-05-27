using Microsoft.EntityFrameworkCore;
using api.Interfaces;
using api.Models;
using api.Data;

namespace api.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDbContext _context;

        public PostRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<Post> GetByIdAsync(int id)
        {
            return _context.Posts.Include(c => c.Comments)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Post?> DeleteAsync(int id)
        {
            var postModel = await _context.Posts.FirstOrDefaultAsync(x => x.Id == id);

            if (postModel == null)
            {
                return null;
            }

            _context.Posts.Remove(postModel);
            await _context.SaveChangesAsync();
            return postModel;
        }

        public Task<bool> PostExists(int id)
        {
            return _context.Posts.AnyAsync(x => x.Id == id);
        }
    }
}