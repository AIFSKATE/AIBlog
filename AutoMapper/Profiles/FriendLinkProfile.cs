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
    public class FriendLinkProfile:Profile
    {
        public FriendLinkProfile()
        {
            CreateMap<FriendLink, FriendLinkDTO>().ReverseMap();
        }
    }
}
