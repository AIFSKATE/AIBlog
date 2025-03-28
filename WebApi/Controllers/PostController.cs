using AutoMapper;
using Domain.Post;
using EFCore;
using EFCore.Data;
using Mapper.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[Action]")]
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
        public async Task<IActionResult> CreatePost(PostDTO postDTO)
        {
            var post = mapper.Map<Post>(postDTO);
            dbContext.Add(post);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<QueryPostDto>> QueryPosts([FromQuery] PagingInput input)
        {
            int cnt = dbContext.posts.Count();
            var list = dbContext.posts.AsNoTracking()
                .Where(x => x.IsDeleted == 0)
                .OrderBy(x => x.CreationTime)
                .Skip((input.Page - 1) * input.Limit)
                .Take(input.Limit);
            var res = list.Select(p => mapper.Map<PostBriefDto>(p)).ToList();
            await Task.CompletedTask;
            return Ok(new QueryPostDto(cnt, res));

        }
    }
}
