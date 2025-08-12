using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.DTO
{
    public class QueryPostDto
    {
        public int Count { get; set; }

        public List<PostBriefDto> Posts { get; set; }

        public string Info { get; set; } = string.Empty;

        public QueryPostDto(
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
