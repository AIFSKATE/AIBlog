using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Identity;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[Action]")]
    public class AccountController : ControllerBase
    {
        RoleManager<Role> roleManager;
        UserManager<User> userManager;

        public AccountController(RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
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
    }
}
