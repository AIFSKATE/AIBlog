using EFCore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.DTO
{
    public class PostDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Html { get; set; }
        public List<TagDTO> Tags { get; set; } = new();
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
