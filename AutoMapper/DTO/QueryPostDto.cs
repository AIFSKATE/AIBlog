using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.DTO
{
    public class QueryPostDto
    {
        /// <summary>
        /// 年份
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Posts
        /// </summary>
        public List<PostBriefDto> Posts { get; set; }

        public QueryPostDto(
            int Count,
            List<PostBriefDto> Posts
            )
        {
            this.Count = Count;
            this.Posts = Posts;
        }
    }
}
