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
    public class TagController : ControllerBase
    {
        readonly IMapper mapper;
        readonly AIBlogDbContext dbContext;
        readonly ILogger<FriendLinkController> logger;

        public TagController(IMapper mapper,
            AIBlogDbContext dbContext,
            ILogger<FriendLinkController> logger)
        {
            this.mapper = mapper;
            this.dbContext = dbContext;
            this.logger = logger;
        }


        [HttpPost]
        public async Task<IActionResult> AddTag(string tagName)
        {
            if (dbContext.tags.Any(t => t.TagName == tagName))
            {
                return BadRequest("This tag already exists");
            }

            dbContext.tags.Add(new Tag { TagName = tagName });
            logger.LogInformation($"Admin {User.Identity?.Name} added a new tag: {tagName}");
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTag(int tagId)
        {
            Tag? tag = dbContext.tags.SingleOrDefault(t => t.Id == tagId);
            if (tag == null)
            {
                return BadRequest("This tag does not exist");
            }
            dbContext.tags.Remove(tag);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> QueryAllTags()
        {
            var list = dbContext.tags.AsNoTracking().ToList();
            var ret = mapper.Map<List<TagDTO>>(list);
            await Task.CompletedTask;
            return Ok(ret);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTag(TagDTO tagDTO)
        {
            Tag? tag = dbContext.tags.SingleOrDefault(t => t.Id == tagDTO.Id);
            if (tag == null)
            {
                return BadRequest("This tag does not exist");
            }
            if (dbContext.tags.Any(t => t.TagName == tagDTO.TagName && t.Id != tagDTO.Id))
            {
                return BadRequest("This tag name already exists");
            }
            tag.TagName = tagDTO.TagName;
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> QueryPostsUnderTag(int tagId, [FromQuery] PagingInput input)
        {
            Tag? tag = dbContext.tags.Include(t => t.Posts).AsNoTracking().SingleOrDefault(t => t.Id == tagId);
            if (tag == null)
            {
                return BadRequest("This tag does not exist");
            }
            var info = tag.TagName;
            var cnt = tag.Posts.Count(p => p.IsDeleted == 0);
            var list = tag.Posts.Where(p => p.IsDeleted == 0)
                .Skip((input.Page - 1) * input.Limit)
                .Take(input.Limit)
                .ToList();
            var ret = mapper.Map<List<PostBriefDto>>(list);
            await Task.CompletedTask;
            return Ok(new QueryPostDto(cnt, ret, info));
        }
    }
}
