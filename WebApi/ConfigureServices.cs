using EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

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
        }
    }
}
