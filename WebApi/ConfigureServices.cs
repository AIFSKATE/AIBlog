using EFCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApi.Identity;

namespace WebApi
{
    public static class ConfigureServices
    {
        public static void ConfigureAIBlogDbContext(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AIBlogDbContext>(opt =>
            {
                string connStr = builder.Configuration.GetConnectionString("mysqlDefaultConnection")!;
                var serverVersion = new MySqlServerVersion(new Version(8, 0, 39));
                opt.UseMySql(connStr, serverVersion);
            });

            builder.Services.AddIdentityCore<User>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 5;
                options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
                options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
            });

            var idBuilder = new IdentityBuilder(typeof(User), typeof(Role), builder.Services);
            idBuilder.AddEntityFrameworkStores<AIBlogDbContext>()
                .AddDefaultTokenProviders()
                .AddRoleManager<RoleManager<Role>>()
                .AddUserManager<UserManager<User>>();
        }
    }
}
