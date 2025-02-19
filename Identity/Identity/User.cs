using Microsoft.AspNetCore.Identity;

namespace WebApi.Identity
{
    public class User : IdentityUser<Guid>
    {
        public DateTime CreationTime { get; set; }
        public string? Introduction { get; set; }
    }
}
