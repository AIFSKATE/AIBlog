using EFCore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.DTO
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; } = string.Empty;
        public int ArticleCount { get; set; }
    }
}
