using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.DTO
{
    public class QueryPostsDto
    {
        public int Count { get; set; }

        public List<PostBriefDto> Posts { get; set; }

        public string Info { get; set; } = string.Empty;

        public QueryPostsDto(
            int Count,
            List<PostBriefDto> Posts,
            string Info= ""
            )
        {
            this.Count = Count;
            this.Posts = Posts;
            this.Info = Info;
        }
    }
}
