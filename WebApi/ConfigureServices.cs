using EFCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApi.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Domain.Account;

namespace WebApi
{
    public static class ConfigureServices
    {
        internal static void ConfigureAIBlogDbContext(WebApplicationBuilder builder)
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

        internal static void ConfigureJWT(WebApplicationBuilder builder)
        {
            var service = builder.Services;
            service.Configure<JWTOptions>(builder.Configuration.GetSection(JWTOptions.JWT));
            service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(x =>
                {
                    var jwtOpt = builder.Configuration.GetSection(JWTOptions.JWT).Get<JWTOptions>()!;
                    var keyBytes = Encoding.UTF8.GetBytes(jwtOpt.SigningKey);
                    var secKey = new SymmetricSecurityKey(keyBytes);
                    x.TokenValidationParameters = new()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = secKey,
                        //ClockSkew = TimeSpan.Zero
                    };
                });
        }

        internal static void ConfigureSwagger(WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(c =>
            {
                var scheme = new OpenApiSecurityScheme()
                {
                    Description = "Authorization header. \r\nExample: 'Bearer 12345abcdef",
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Authorization" },
                    Scheme = "oauth2",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                };
                c.AddSecurityDefinition("Authorization", scheme);
                var requirement = new OpenApiSecurityRequirement();
                requirement[scheme] = new List<string>();
                c.AddSecurityRequirement(requirement);
            });
        }
    }
}
