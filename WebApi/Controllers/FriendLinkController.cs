using AutoMapper;
using EFCore.Data;
using Mapper.DTO;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[Action]")]
    public class FriendLinkController : ControllerBase
    {
        readonly IMapper mapper;

        public FriendLinkController(IMapper mapper)
        {
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> AddLink(FriendLinkDTO friendLinkDTO)
        {
            await Task.CompletedTask;
            var friendlink = mapper.Map<FriendLink>(friendLinkDTO);
            return Ok(friendlink);
        }
    }
}
