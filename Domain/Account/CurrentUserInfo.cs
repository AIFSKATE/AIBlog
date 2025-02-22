using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Account
{
    public class CurrentUserInfo
    {
        public CurrentUserInfo(string id, string userName, IEnumerable<Claim> roles)
        {
            Id = id;
            UserName = userName;
            Roles = roles.Select(r => r.Value).ToList();
        }
        public string Id { get; set; }
        public string UserName { get; set; }
        public List<string> Roles { get; set; }

        //public override string ToString()
        //{
        //    string res = $"Id:{Id}\nUserName:{UserName}\nRoles:";
        //    string rolesString = string.Join(",", Roles.Select(r => r.Value));
        //    return res + rolesString;
        //}
    }
}
