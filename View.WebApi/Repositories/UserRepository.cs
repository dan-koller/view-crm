using View.Shared; // ApplicationUser
using View.WebApi.Data;
using Microsoft.AspNetCore.Identity;

namespace View.WebApi.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public UserRepository(UserContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    // Default roles
    private const string RoleRegisteredUser = "RegisteredUser";
    private const string RoleAdministrator = "Administrator";

    private async Task CreateDefaultRoles()
    {
        if (!await _roleManager.RoleExistsAsync(RoleRegisteredUser))
        {
            await _roleManager.CreateAsync(new IdentityRole(RoleRegisteredUser));
        }
        if (!await _roleManager.RoleExistsAsync(RoleAdministrator))
        {
            await _roleManager.CreateAsync(new IdentityRole(RoleAdministrator));
        }
    }

    public async Task<ApplicationUser?> CreateUserAsync(RegisterRequest registerRequest)
    {
        // create the default roles (if they don't exist yet)
        await CreateDefaultRoles();

        var user = new ApplicationUser()
        {
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = registerRequest.Email,
            Email = registerRequest.Email,
            Name = registerRequest.Name
        };

        var result = await _userManager.CreateAsync(user, registerRequest.Password);

        if (result.Succeeded)
        {
            // the first registered user will be an admin in addition to a registered user
            if (_userManager.Users.Count() == 1)
            {
                await _userManager.AddToRoleAsync(user, RoleAdministrator);
                await _userManager.AddToRoleAsync(user, RoleRegisteredUser);
            }
            else
            {
                await _userManager.AddToRoleAsync(user, RoleRegisteredUser);
            }

            // confirm the e-mail and remove lockout
            user.EmailConfirmed = true;
            user.LockoutEnabled = false;

            // update changes
            await _userManager.UpdateAsync(user);

            return user;
        }
        else
        {
            return null;
        }
    }
}

