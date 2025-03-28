using AutoMapper;
using EFCore.Data;
using Mapper.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Profiles
{
    public class AIBlogProfile:Profile
    {
        public AIBlogProfile()
        {
            CreateMap<FriendLink, FriendLinkDTO>().ReverseMap();
            CreateMap<Post, PostDTO>().ReverseMap();
            CreateMap<Post, PostBriefDto>();
            CreateMap<Tag, TagDTO>().ReverseMap();
        }
    }
}
