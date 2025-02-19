using EFCore.Data;
using Microsoft.EntityFrameworkCore;

namespace EFCore
{
    public class AIBlogDbContext : DbContext
    {
        public DbSet<Post> posts { get; set; }
        public DbSet<Tag> tags { get; set; }
        public AIBlogDbContext(DbContextOptions<AIBlogDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }
    }
}
