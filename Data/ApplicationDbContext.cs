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

        public DbSet<UserProfile> UserProfiles { get; set; }

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

            builder.Entity<User>()
                .HasOne(u => u.DisplayPicture)
                .WithOne(dp => dp.User)
                .HasForeignKey<DisplayPicture>(dp => dp.UserId)
                .OnDelete(DeleteBehavior.Cascade); 

            builder.Entity<DisplayPicture>()
                .HasMany(dp => dp.Likes)
                .WithMany()
                .UsingEntity(j => j.ToTable("DisplayPictureLikes"));

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

            builder.Entity<User>()
                .HasOne<UserProfile>()
                .WithOne()
                .HasForeignKey<UserProfile>(up => up.UserId);

            builder.Entity<UserProfile>()
                .HasKey(up => up.UserId); 

            builder.Entity<UserProfile>()
                .OwnsOne(up => up.Visibility, vs =>
                {
                    vs.Property(v => v.Birthday).HasColumnName("Visibility_Birthday");
                    vs.Property(v => v.Hometown).HasColumnName("Visibility_Hometown");
                    vs.Property(v => v.Occupation).HasColumnName("Visibility_Occupation");
                });

            builder.Entity<Post>()
                .HasMany(p => p.Likes)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "PostLikes",
                    j => j.HasOne<User>().WithMany().HasForeignKey("UserId"),
                    j => j.HasOne<Post>().WithMany().HasForeignKey("PostId"));

            builder.Entity<Comment>()
                .HasMany(c => c.Likes)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "CommentLikes",
                    j => j.HasOne<User>().WithMany().HasForeignKey("UserId"),
                    j => j.HasOne<Comment>().WithMany().HasForeignKey("CommentId"));
                    }
    }
}
