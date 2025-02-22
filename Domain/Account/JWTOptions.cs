using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Account
{
    public class JWTOptions
    {
        public const string JWT = "Jwt";
        public string SigningKey { get; set; }
        public int ExpireSeconds { get; set; }
    }
}
