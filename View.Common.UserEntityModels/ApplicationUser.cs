using Microsoft.AspNetCore.Identity;

namespace View.Shared;

// The same model is used for both the Sqlite and SqlServer databases
public class ApplicationUser : IdentityUser
{
    public string? Name { get; set; }
}