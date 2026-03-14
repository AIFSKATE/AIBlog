using AutoMapper;
using AutoMapper.QueryableExtensions;
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
            return Ok(await GetPagedPosts(input, 0));
        }

        [HttpGet]
        public async Task<IActionResult> AdminQueryPosts([FromQuery] PagingInput input, int isDeleted)
        {
            return Ok(await GetPagedPosts(input, isDeleted));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPost(int postId)
        {
            var post = await dbContext.posts.Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.Id == postId && p.IsDeleted == 0);

            return await ProcessPostResponse(post);
        }

        [HttpGet]
        public async Task<IActionResult> AdminGetPost(int postId)
        {
            var post = await dbContext.posts.Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.Id == postId);

            return await ProcessPostResponse(post);
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

        [HttpPut("{postId}")]
        public async Task<IActionResult> RestorePost(int postId)
        {
            // 查询文章（包括已删除的）
            Post? post = dbContext.posts.SingleOrDefault(p => p.Id == postId);

            if (post == null)
            {
                return BadRequest("This post does not exist");
            }

            // 将状态改回 0
            post.IsDeleted = 0;

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

        private async Task<QueryPostsDto> GetPagedPosts(PagingInput input, int isDeleted)
        {
            // 1. 参数防御
            int page = input.Page < 1 ? 1 : input.Page;
            int limit = input.Limit < 1 ? 10 : (input.Limit > 100 ? 100 : input.Limit);

            // 2. 构造基础查询
            var query = dbContext.posts
                .AsNoTracking()
                .Where(x => x.IsDeleted == isDeleted);

            // 3. 异步获取总数
            int totalCount = await query.CountAsync();

            // 4. 使用 ProjectTo 进行数据库级别的投影，并使用 ToListAsync
            var list = await query
                .OrderByDescending(x => x.CreationTime)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ProjectTo<PostBriefDto>(mapper.ConfigurationProvider) // 优化查询性能
                .ToListAsync(); // 异步获取列表

            return new QueryPostsDto(totalCount, list);
        }

        private async Task<IActionResult> ProcessPostResponse(Post? post)
        {
            if (post == null)
            {
                return BadRequest("This post does not exist");
            }

            var ret = mapper.Map<PostDTO>(post);
            if (ret.CategoryId != null)
            {
                // 建议使用 FindAsync 或 FirstOrDefaultAsync，并在分类不存在时做防御
                var category = await dbContext.categories.FindAsync(ret.CategoryId);
                ret.CategoryName = category?.CategoryName ?? "Unknown";
            }
            return Ok(ret);
        }
    }
}
