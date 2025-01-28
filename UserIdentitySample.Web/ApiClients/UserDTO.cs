using Microsoft.AspNetCore.Identity;

namespace UserIdentitySample.Web.ApiClients
{
    public class UserDTO : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
