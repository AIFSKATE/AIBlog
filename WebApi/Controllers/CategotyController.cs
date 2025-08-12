using AutoMapper;
using EFCore.Data;
using EFCore;
using Mapper.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Account;
using Domain.Post;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[Action]")]
    [Authorize(Roles = AIBlogRole.Admin)]
    public class CategotyController : ControllerBase
    {
        readonly IMapper mapper;
        readonly ILogger<CategotyController> logger;
        readonly AIBlogDbContext dbContext;

        public CategotyController(IMapper mapper,
            AIBlogDbContext dbContext,
            ILogger<CategotyController> logger)
        {
            this.mapper = mapper;
            this.dbContext = dbContext;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryCreation categoryCreation)
        {
            var category = mapper.Map<Category>(categoryCreation);
            dbContext.categories.Add(category);
            await dbContext.SaveChangesAsync();
            logger.LogTrace($"Category added: {category.CategoryName}");
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> QueryCategories()
        {
            var list = dbContext.categories.AsNoTracking()
                .Where(f => f.IsDeleted == 0).ToList();
            var ret = mapper.Map<List<CategoryDTO>>(list);
            await Task.CompletedTask;
            return Ok(ret);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            Category? category = dbContext.categories.SingleOrDefault(f => f.Id == categoryId);
            if (category == null)
            {
                return BadRequest("This category does not exist");
            }
            category.IsDeleted = 1;
            logger.LogTrace($"Category deleted: {category.CategoryName}");
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCategory(CategoryDTO categoryDTO)
        {
            Category? category = dbContext.categories.SingleOrDefault(f => f.Id == categoryDTO.Id);
            if (category == null)
            {
                return BadRequest("This category does not exist");
            }
            mapper.Map(categoryDTO, category);
            await dbContext.SaveChangesAsync();
            logger.LogTrace($"Category updated: {category.CategoryName}");
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> QueryPostsUnderCategory([FromQuery] PagingInput input, int categoryId)
        {
            var posts = dbContext.categories
                .Where(c => c.Id == categoryId && c.IsDeleted == 0);
            if (!posts.Any() || posts == null)
            {
                return NotFound("This category does not exist or no posts found under this category.");
            }
            var allPosts = posts.Include(c => c.Posts).SelectMany(c => c.Posts).Where(p => p.IsDeleted == 0);
            var cnt = allPosts.Count();

            var ret = allPosts
                .Skip((input.Page - 1) * input.Limit)
                .Take(input.Limit)
                .ToList();
            var res = mapper.Map<List<PostBriefDto>>(ret);
            await Task.CompletedTask;
            return Ok(new QueryPostDto(cnt, res));
        }
    }
}
