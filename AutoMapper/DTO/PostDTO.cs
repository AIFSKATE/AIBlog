using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.DTO
{
    public class PostDTO
    {
        public string Title { get; set; }
        public string Html { get; set; }
        public List<TagDTO> Tags { get; set; } = new List<TagDTO>();
        public DateTime CreationTime { get; set; }
    }
}
