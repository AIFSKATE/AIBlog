using AutoMapper;
using Domain.Account;
using Domain.Post;
using EFCore;
using EFCore.Data;
using Mapper.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[Action]")]
    [Authorize(Roles = AIBlogRole.Admin)]
    public class PostController : ControllerBase
    {
        readonly IMapper mapper;
        readonly IMemoryCache memoryCache;
        readonly AIBlogDbContext dbContext;

        public PostController(IMapper mapper,
            IMemoryCache memoryCache,
            AIBlogDbContext dbContext)
        {
            this.mapper = mapper;
            this.memoryCache = memoryCache;
            this.dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(PostCreation postcreation)
        {
            var post = mapper.Map<Post>(postcreation);
            var categoryID = postcreation?.CategoryId;
            if (categoryID != null)
            {
                var category = await dbContext.categories.SingleOrDefaultAsync(c => c.Id == categoryID);
                if (category != null)
                {
                    post.Category = category;
                }
                else
                {
                    return BadRequest("This category does not exist");
                }
            }
            post.Tags = dbContext.tags.Where(t => postcreation!.TagIDs.Contains(t.Id)).ToList();

            dbContext.Add(post);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> QueryPosts([FromQuery] PagingInput input)
        {
            int cnt = dbContext.posts.Count(x => x.IsDeleted == 0);
            var list = dbContext.posts.AsNoTracking()
                .Where(x => x.IsDeleted == 0)
                .OrderBy(x => x.CreationTime)
                .Skip((input.Page - 1) * input.Limit)
                .Take(input.Limit);
            var res = list.Select(p => mapper.Map<PostBriefDto>(p)).ToList();
            await Task.CompletedTask;
            return Ok(new QueryPostDto(cnt, res));

        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPost(int postId)
        {
            var post = await dbContext.posts.Include(p => p.Tags)
                .SingleOrDefaultAsync(p => p.Id == postId && p.IsDeleted == 0);
            if (post == null)
            {
                return BadRequest("This post does not exist");
            }
            var ret = mapper.Map<PostDTO>(post);
            if (ret.CategoryId != null)
            {
                ret.CategoryName = dbContext.categories.Single(c => c.Id == ret.CategoryId).CategoryName;
            }
            return Ok(ret);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePost(PostDTO postDTO)
        {
            Post? post = dbContext.posts.Include(p => p.Tags).SingleOrDefault(p => p.Id == postDTO.Id);
            if (post == null)
            {
                return BadRequest("This post does not exist");
            }
            mapper.Map<PostDTO, Post>(postDTO, post);

            post.Tags = dbContext.tags.Where(t => postDTO.Tags.Select(t => t.Id).Contains(t.Id)).ToList();

            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePost(int postId)
        {
            Post? post = dbContext.posts.SingleOrDefault(p => p.Id == postId);
            if (post == null)
            {
                return BadRequest("This post does not exist");
            }
            post.IsDeleted = 1;
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
