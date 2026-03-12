using AutoMapper;
using Domain.Post;
using EFCore.Data;
using Mapper.DTO;

namespace Mapper.Profiles
{
    public class AIBlogProfile : Profile
    {
        public AIBlogProfile()
        {
            CreateMap<FriendLink, FriendLinkDTO>().ReverseMap();
            CreateMap<FriendLinkCreation, FriendLink>();

            CreateMap<Post, PostDTO>()
                .ForMember(p => p.CategoryName, opt => opt.Ignore());
            CreateMap<PostDTO, Post>()
                .ForMember(p => p.CreationTime, opt => opt.Ignore())
                .ForMember(p => p.Tags, opt => opt.Ignore());
            CreateMap<Post, PostBriefDto>();
            CreateMap<PostCreation, Post>();
            CreateMap<PostDTO, PostCreation>()
                .ForMember(p => p.TagIDs, opt => opt.MapFrom(src => src.Tags != null ? src.Tags.Select(t => t.Id).ToList() : new List<int>()));

            CreateMap<Tag, TagDTO>().ReverseMap();


            CreateMap<Category, CategoryDTO>()
                .ForMember(p => p.ArticleCount, opt => opt.MapFrom(src => src.Posts.Count));
            CreateMap<CategoryDTO, Category>();
        }
    }
}
