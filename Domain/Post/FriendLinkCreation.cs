using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Post
{
    public class FriendLinkCreation
    {
        public string Title { get; set; }

        public string LinkUrl { get; set; }

        public FriendLinkCreation(string title, string linkUrl)
        {
            Title = title;
            LinkUrl = linkUrl;
        }

        public FriendLinkCreation()
        {
            Title = string.Empty;
            LinkUrl = string.Empty;
        }
    }
}
