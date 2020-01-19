
using Microsoft.EntityFrameworkCore;
using YAHCMS.BlogService.Models;

namespace YAHCMS.BlogService.Persistence
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options) :
            base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            
        }
        public DbSet<Blog> blogs {get; set;}
        public DbSet<Post> posts {get; set;}
    }
}