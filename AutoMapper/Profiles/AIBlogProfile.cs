using AutoMapper;
using Domain.Post;
using EFCore.Data;
using Mapper.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Profiles
{
    public class AIBlogProfile : Profile
    {
        public AIBlogProfile()
        {
            CreateMap<FriendLink, FriendLinkDTO>().ReverseMap();
            CreateMap<Post, PostDTO>().ReverseMap();
            CreateMap<Post, PostBriefDto>().ReverseMap();

            CreateMap<PostCreation, Post>();
            CreateMap<FriendLinkCreation, FriendLink>();
            CreateMap<CategoryCreation, Category>();

            CreateMap<Tag, TagDTO>().ReverseMap();
            CreateMap<Category, CategoryDTO>().ReverseMap();
        }
    }
}
