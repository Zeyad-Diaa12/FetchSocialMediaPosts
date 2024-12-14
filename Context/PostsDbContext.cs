using FetchSocialMediaPosts.DTO;
using FetchSocialMediaPosts.Platforms.Facebook;
using FetchSocialMediaPosts.Platforms.Twitter;
using Microsoft.EntityFrameworkCore;

namespace FetchSocialMediaPosts.Context
{
    public class PostsDbContext : DbContext
    {
        public PostsDbContext(DbContextOptions<PostsDbContext> dbContextOptions)
            : base(dbContextOptions)
        {
        }

        public DbSet<TwitterPost> Twitter { get; set; }
        public DbSet<FacebookPost> Facebook { get; set; }
        public DbSet<InstagramPost> Instagram { get; set; }
    }
}
