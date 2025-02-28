using EFCore.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApi.Identity;

namespace EFCore
{
    public class AIBlogDbContext : IdentityDbContext<User, Role, Guid>
    {
        public DbSet<Post> posts { get; set; }
        public DbSet<Tag> tags { get; set; }
        public DbSet<FriendLink> friends { get; set; }
        public DbSet<Category> categories { get; set; }

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
