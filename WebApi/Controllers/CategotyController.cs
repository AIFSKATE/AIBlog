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

            var existingCategory = await dbContext.categories
                .FirstOrDefaultAsync(c => c.CategoryName == category.CategoryName);

            if (existingCategory != null)
            {
                if (existingCategory.IsDeleted == 0)
                {
                    return BadRequest("This category already exists and is not deleted");
                }
                else
                {
                    existingCategory.IsDeleted = 0;
                    // Use existing mapper pattern to update the restored category's description/details
                    mapper.Map(categoryDTO, existingCategory);
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

        [HttpDelete]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            return await UpdateDeleteStatus(categoryId, 1);
        }

        [HttpPut]
        public async Task<IActionResult> RestoreCategory(int categoryId)
        {
            return await UpdateDeleteStatus(categoryId, 0);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> QueryCategories()
        {
            var result = await GetCategoriesInternal(0);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult> AdminQueryCategories(int isDeleted = 0)
        {
            var result = await GetCategoriesInternal(isDeleted);
            return Ok(result);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> QueryPostsUnderCategory([FromQuery] PagingInput input, int categoryId)
        {
            return await GetPostsUnderCategoryInternal(input, categoryId, checkDeleted: true);
        }

        [HttpGet]
        public async Task<IActionResult> AdminQueryPostsUnderCategory([FromQuery] PagingInput input, int categoryId)
        {
            return await GetPostsUnderCategoryInternal(input, categoryId, checkDeleted: false);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCategory(CategoryDTO categoryDTO)
        {
            var category = await dbContext.categories.FirstOrDefaultAsync(f => f.Id == categoryDTO.Id);
            if (category == null)
            {
                return BadRequest("This category does not exist");
            }

            var nameExists = await dbContext.categories
                .AnyAsync(c => c.CategoryName == categoryDTO.CategoryName && c.Id != categoryDTO.Id);

            if (nameExists)
            {
                return BadRequest("This category name already exists");
            }

            mapper.Map(categoryDTO, category);
            await dbContext.SaveChangesAsync();
            logger.LogTrace($"Category updated: {category.CategoryName}");
            return Ok();
        }

        private async Task<IActionResult> UpdateDeleteStatus(int id, int status)
        {
            var category = await dbContext.categories.FirstOrDefaultAsync(f => f.Id == id);
            if (category == null)
            {
                return BadRequest("This category does not exist");
            }
            category.IsDeleted = status;

            var associatedPosts = await dbContext.posts
                .Where(p => p.CategoryId == id)
                .ToListAsync();
            foreach (var post in associatedPosts)
            {
                post.IsDeleted = status;
            }
            await dbContext.SaveChangesAsync();
            logger.LogTrace($"Category '{category.CategoryName}' and {associatedPosts.Count} posts updated to IsDeleted = {status}");
            return Ok();
        }

        private async Task<List<CategoryDTO>> GetCategoriesInternal(int isDeletedFilter)
        {
            var query = dbContext.categories.AsNoTracking();

            query = query.Where(f => f.IsDeleted == isDeletedFilter);

            return await query.Select(c => new CategoryDTO
            {
                Id = c.Id,
                CategoryName = c.CategoryName,
                Description = c.Description,
                ArticleCount = c.Posts.Count(p => p.IsDeleted == 0)
            }).ToListAsync();
        }

        private async Task<IActionResult> GetPostsUnderCategoryInternal(PagingInput input, int categoryId, bool checkDeleted)
        {
            var categoryQuery = dbContext.categories.AsNoTracking().Where(c => c.Id == categoryId);

            if (checkDeleted)
            {
                categoryQuery = categoryQuery.Where(c => c.IsDeleted == 0);
            }

            var categoryName = await categoryQuery.Select(c => c.CategoryName).FirstOrDefaultAsync();

            if (categoryName == null)
            {
                return NotFound("This category does not exist");
            }

            var query = dbContext.posts.AsNoTracking().Where(p => p.CategoryId == categoryId);


            // 2. Only add the IsDeleted filter if required (no else needed)
            if (checkDeleted)
            {
                query = query.Where(p => p.IsDeleted == 0);
            }


            var cnt = await query.CountAsync();

            var list = await query
                .OrderByDescending(p => p.CreationTime)
                .Skip((input.Page - 1) * input.Limit)
                .Take(input.Limit)
                .ToListAsync();

            var res = mapper.Map<List<PostBriefDto>>(list);

            return Ok(new QueryPostsDto(cnt, res, categoryName));
        }
    }
}
