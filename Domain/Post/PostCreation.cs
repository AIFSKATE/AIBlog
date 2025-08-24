using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Post
{
    public class PostCreation
    {
        public string Title { get; set; }
        public string Markdown { get; set; }
        public List<int> TagIDs { get; set; } = new List<int>();
        public int? CategoryId { get; set; }
    }
}
