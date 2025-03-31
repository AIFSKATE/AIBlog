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
        public string Html { get; set; }
        public List<int> TagIDs { get; set; } = new List<int>();
        public int? CategoryID { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
