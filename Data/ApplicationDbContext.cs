using Microsoft.EntityFrameworkCore;
using socialApi.Models;

namespace socialApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
        {

        }

        public DbSet<Comment> Comment { get; set; }
        public DbSet<DisplayPicture> DisplayPicture { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<User> User { get; set; }
    }
}