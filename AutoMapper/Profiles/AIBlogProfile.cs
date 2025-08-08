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
            CreateMap<FriendLinkCreation, FriendLink>();

            CreateMap<Post, PostDTO>().ReverseMap();
            CreateMap<Post, PostBriefDto>().ReverseMap();
            CreateMap<PostCreation, Post>();

            CreateMap<Tag, TagDTO>().ReverseMap();


            CreateMap<CategoryCreation, Category>();
            CreateMap<Category, CategoryDTO>().ReverseMap();
        }
    }
}
