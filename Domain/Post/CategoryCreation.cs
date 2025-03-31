using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Post
{
    public class CategoryCreation
    {
        public string CategoryName { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<int> PostIDs { get; set; }

    }
}
