using Microsoft.AspNetCore.Identity;

namespace UserIdentitySample.ApiService.Domain;

public class User : IdentityUser
{
    public string? FullName { get; set; }
}
