using AutoMapper;
using Domain.Account;
using Domain.Post;
using EFCore;
using EFCore.Data;
using Mapper.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[Action]")]
    [Authorize(Roles = AIBlogRole.Admin)]
    public class FriendLinkController : ControllerBase
    {
        readonly IMapper mapper;
        readonly AIBlogDbContext dbContext;
        readonly ILogger<FriendLinkController> logger;


        public FriendLinkController(IMapper mapper,
            AIBlogDbContext dbContext,
            ILogger<FriendLinkController> logger)
        {
            this.mapper = mapper;
            this.dbContext = dbContext;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> AddLink(FriendLinkCreation friendLinkCreation)
        {
            var friendlink = mapper.Map<FriendLink>(friendLinkCreation);
            dbContext.friends.Add(friendlink);
            logger.LogInformation($"Admin {User.Identity?.Name} added a new friend link: {friendLinkCreation.Title} - {friendLinkCreation.LinkUrl}");
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> QueryFriendLinks()
        {
            var list = dbContext.friends.AsNoTracking()
                .Where(f => f.IsDeleted == 0)
                .Select(f => mapper.Map<FriendLinkDTO>(f)).ToList();
            await Task.CompletedTask;
            return Ok(list);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFriendLink(int friendLinkId)
        {
            FriendLink? friendLink = dbContext.friends.SingleOrDefault(f => f.Id == friendLinkId);
            if (friendLink == null)
            {
                return BadRequest("This FriendLink does not exist");
            }
            friendLink.IsDeleted = 1;
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateFriendLink(FriendLinkDTO friendLinkDTO)
        {
            FriendLink? friendLink = dbContext.friends.SingleOrDefault(f => f.Id == friendLinkDTO.Id);
            if (friendLink == null)
            {
                return BadRequest("This FriendLink does not exist");
            }
            mapper.Map<FriendLinkDTO, FriendLink>(friendLinkDTO, friendLink);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
