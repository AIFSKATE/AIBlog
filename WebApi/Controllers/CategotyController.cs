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
        readonly AIBlogDbContext dbContext;

        public CategotyController(IMapper mapper,
            AIBlogDbContext dbContext)
        {
            this.mapper = mapper;
            this.dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryCreation categoryCreation)
        {
            var category = mapper.Map<Category>(categoryCreation);
            category.Posts = dbContext.posts.Where(p => categoryCreation.PostIDs.Contains(p.Id)).ToList();
            dbContext.categories.Add(category);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> QueryCategories()
        {
            var list = dbContext.friends.AsNoTracking()
                .Where(f => f.IsDeleted == 0)
                .Select(f => mapper.Map<CategoryDTO>(f)).ToList();
            await Task.CompletedTask;
            return Ok(list);
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
            return Ok();
        }
    }
}
