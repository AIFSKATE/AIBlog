using AutoMapper;
using Mapper.DTO;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[Action]")]
    public class PostController : ControllerBase
    {
        readonly IMapper mapper;

        public PostController(IMapper mapper)
        {
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(PostDTO postDTO)
        {
            await Task.CompletedTask;
            return Ok();
        }
    }
}
