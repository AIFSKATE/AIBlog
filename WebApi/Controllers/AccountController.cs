using Domain.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Filters;
using WebApi.Identity;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[Action]")]
    [Authorize(Roles = AIBlogRole.Admin)]
    [TypeFilter<AccountExceptioncs>]
    public class AccountController : ControllerBase
    {
        readonly RoleManager<Role> roleManager;
        readonly UserManager<User> userManager;
        readonly IConfiguration configuration;
        readonly JWTOptions jwtOpt;
        readonly ILogger<AccountController> logger;

        public AccountController(RoleManager<Role> roleManager,
            UserManager<User> userManager,
            IConfiguration configuration,
            IOptionsSnapshot<JWTOptions> jwtOpt,
            ILogger<AccountController> logger)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.configuration = configuration;
            this.jwtOpt = jwtOpt.Value;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterInfo registerInfo)
        {
            string username = registerInfo.Name;
            string password = registerInfo.Password;

            var user = await userManager.FindByNameAsync(username);
            if (user != null)
            {
                return BadRequest("This user already exists");
            }

            user = new User()
            {
                UserName = username,
                Email = registerInfo.Email,
                CreationTime = DateTime.Now,
            };
            var createUser = await userManager.CreateAsync(user, password);
            if (!createUser.Succeeded)
            {
                return BadRequest(createUser.Errors);
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleCreateInfo roleCreateInfo)
        {
            var exist = await roleManager.RoleExistsAsync(roleCreateInfo.RoleName);
            if (exist)
            {
                return BadRequest("This role already exists");
            }
            Role role = new Role()
            {
                Name = roleCreateInfo.RoleName
            };
            var createRole = await roleManager.CreateAsync(role);
            if (!createRole.Succeeded)
            {
                return BadRequest(createRole.Errors);
            }
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> UserToRole(string username, string rolename)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound("User does not exist");
            }
            var role = await roleManager.RoleExistsAsync(rolename);
            if (!role)
            {
                return NotFound("Role does not exist");
            }
            var isInRole = await userManager.IsInRoleAsync(user, rolename);
            if (!isInRole)
            {
                var userToRole = await userManager.AddToRoleAsync(user, rolename);
                if (!userToRole.Succeeded)
                {
                    return BadRequest(userToRole.Errors);
                }
            }
            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginInfo loginInfo)
        {
            logger.LogTrace($"{loginInfo.Name} tries to login");
            string userName = loginInfo.Name;
            string password = loginInfo.Password;
            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return NotFound($"{userName} doesn't exist.");
            }
            if (await userManager.IsLockedOutAsync(user))
            {
                return Forbid($"{userName} is locked");
            }
            var success = await userManager.CheckPasswordAsync(user, password);
            if (success)
            {
                await userManager.ResetAccessFailedCountAsync(user);

                // 1 定义需要的Cliam信息
                var claims = new List<Claim>(){
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name,user.UserName!),
                };
                var list = await userManager.GetRolesAsync(user);
                foreach (var role in list)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
                //var jwtOpt = configuration.GetSection("Jwt").Get<JWTOptions>()!;

                // 2 设置SecretKey
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOpt.SigningKey));

                // 3 设置加密算法
                var algorithm = SecurityAlgorithms.HmacSha256;

                // 4 生成签名凭证信息
                var signingCredentials = new SigningCredentials(secretKey, algorithm);

                // 5 设置Token过期时间
                var expires = DateTime.Now.AddSeconds(jwtOpt.ExpireSeconds);

                // 6 生成token
                var securityToken = new JwtSecurityToken(
                    claims: claims,
                    expires: expires,
                    signingCredentials: signingCredentials
                );

                var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
                var token = jwtSecurityTokenHandler.WriteToken(securityToken);

                logger.LogTrace($"{loginInfo.Name} login succesfully");

                return Ok(token);
            }
            else
            {
                await userManager.AccessFailedAsync(user);
                return BadRequest("Password is incorrect.");
            }
        }

        [HttpGet]
        public async Task<ActionResult<CurrentUserInfo>> CurrentUser()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Unauthorized(new { message = "用户未登录或会话已过期" });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName))
            {
                return BadRequest("用户信息不完整，请重新登录");
            }

            var roleClaims = User.FindAll(ClaimTypes.Role);

            // 5. 此时再构造对象就是绝对安全的
            var userInfo = new CurrentUserInfo(userId, userName, roleClaims);

            return Ok(userInfo);
        }

        [HttpGet]
        public async Task<ActionResult<List<string>>> GetRoles()
        {
            var list = await roleManager.Roles.ToListAsync();
            return Ok(list);
        }

        [HttpGet]
        public string Exception()
        {
            throw new NotImplementedException("这是一个未实现的异常接口");
        }
    }
}
