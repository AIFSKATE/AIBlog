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
    public class CategoryController : ControllerBase
    {
        readonly IMapper mapper;
        readonly ILogger<CategoryController> logger;
        readonly AIBlogDbContext dbContext;

        public CategoryController(IMapper mapper,
            AIBlogDbContext dbContext,
            ILogger<CategoryController> logger)
        {
            this.mapper = mapper;
            this.dbContext = dbContext;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryDTO categoryDTO)
        {
            var category = mapper.Map<Category>(categoryDTO);
            category.Id = 0;
            var existingCategory = dbContext.categories
                .SingleOrDefault(c => c.CategoryName == category.CategoryName);

            if (existingCategory != null)
            {
                if (existingCategory.IsDeleted == 0)
                {
                    return BadRequest("This category already exists and is not deleted");
                }
                else
                {
                    existingCategory.IsDeleted = 0; // Restore the deleted category
                }
            }
            else
            {
                dbContext.categories.Add(category);
            }
            await dbContext.SaveChangesAsync();
            logger.LogTrace($"Category added: {category.CategoryName}");
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> QueryCategories()
        {
            var list = dbContext.categories.Include(c => c.Posts).AsNoTracking()
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
            else if (dbContext.categories.Any(c => c.CategoryName == categoryDTO.CategoryName))
            {
                return BadRequest("This category name already exists");
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

            var info = posts.FirstOrDefault()?.CategoryName!;

            var allPosts = posts.Include(c => c.Posts).SelectMany(c => c.Posts).Where(p => p.IsDeleted == 0);
            var cnt = allPosts.Count();

            var ret = allPosts
                .Skip((input.Page - 1) * input.Limit)
                .Take(input.Limit)
                .ToList();
            var res = mapper.Map<List<PostBriefDto>>(ret);
            await Task.CompletedTask;
            return Ok(new QueryPostsDto(cnt, res, info));
        }
    }
}
