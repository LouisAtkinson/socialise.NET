using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using api.Models;

namespace api.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<DisplayPicture> DisplayPictures { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Friendship> Friendships { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = "1",  
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "1"  
                },
                new IdentityRole
                {
                    Id = "2",
                    Name = "User",
                    NormalizedName = "USER",
                    ConcurrencyStamp = "2"  
                },
            };
            builder.Entity<IdentityRole>().HasData(roles);

            builder.Entity<Friendship>()
               .HasOne(f => f.UserA)
               .WithMany()
               .HasForeignKey(f => f.UserAId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Friendship>()
                .HasOne(f => f.UserB)
                .WithMany()
                .HasForeignKey(f => f.UserBId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
