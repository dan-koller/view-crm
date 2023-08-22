using Microsoft.AspNetCore.Identity;

namespace View.Shared;

public class ApplicationUser : IdentityUser
{
    public string? Name { get; set; }
}