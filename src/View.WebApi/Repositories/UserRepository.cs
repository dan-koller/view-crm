using Microsoft.AspNetCore.Identity;
using View.Shared; // ApplicationUser
using View.WebApi.Data;

namespace View.WebApi.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // Default roles
    private const string RoleRegisteredUser = "RegisteredUser";
    private const string RoleAdministrator = "Administrator";

    // Create default roles if they don't exist
    private async Task CreateDefaultRoles()
    {
        if (!await _roleManager.RoleExistsAsync(RoleRegisteredUser))
            await _roleManager.CreateAsync(new IdentityRole(RoleRegisteredUser));
        if (!await _roleManager.RoleExistsAsync(RoleAdministrator))
            await _roleManager.CreateAsync(new IdentityRole(RoleAdministrator));
    }

    public async Task<ApplicationUser?> CreateUserAsync(RegisterRequest registerRequest)
    {
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

            // bypass account confirmation because there is no email service implemented yet
            user.EmailConfirmed = true;
            user.LockoutEnabled = false;

            await _userManager.UpdateAsync(user);

            return user;
        }
        else
        {
            return null;
        }
    }

    public async Task<ApplicationUser?> UpdateUserAsync(UpdateAccountRequest updateRequest, ApplicationUser user)
    {
        bool hasNameChanged = user.Name != updateRequest.Name;
        if (hasNameChanged && !string.IsNullOrWhiteSpace(updateRequest.Name))
        {
            user.Name = updateRequest.Name;
            await _userManager.UpdateAsync(user);
        }

        if (!string.IsNullOrWhiteSpace(updateRequest.NewPassword))
        {
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, updateRequest.CurrentPassword, updateRequest.NewPassword);

            if (!changePasswordResult.Succeeded) return null;
        }

        return user;
    }
}

