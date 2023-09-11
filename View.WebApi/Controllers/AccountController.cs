using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using View.WebApi.Data;
using View.Shared; // ApplicationUser
using Microsoft.AspNetCore.Authorization;

namespace View.WebApi.Controllers;

// base address: api/account
[Route(Constants.ApiRoute)]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtHandler _jwtHandler;

    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AccountController(UserContext context, UserManager<ApplicationUser> userManager, JwtHandler jwtHandler, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
        _context = context;
        _userManager = userManager;
        _jwtHandler = jwtHandler;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    // TODO: Move the logic to the UserRepository class

    // POST: api/account/login
    [HttpPost("login")]
    [ProducesResponseType(200, Type = typeof(string))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        var user = await _userManager.FindByNameAsync(loginRequest.Email);
        if (user == null
            || !await _userManager.CheckPasswordAsync(user, loginRequest.Password))
            return Unauthorized(new LoginResult()
            {
                Success = false,
                Message = "Invalid Email or Password."
            });
        var secToken = await _jwtHandler.GetTokenAsync(user);
        secToken.Payload["Name"] = user.Name;
        var jwt = new JwtSecurityTokenHandler().WriteToken(secToken);
        return Ok(new LoginResult()
        {
            Success = true,
            Message = "Login successful",
            Token = jwt
        });
    }

    // POST: api/account/register
    [HttpPost("register")]
    [ProducesResponseType(200, Type = typeof(string))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Register(RegisterRequest registerRequest)
    {
        var user = await _userManager.FindByNameAsync(registerRequest.Email);
        if (user != null)
            return Unauthorized(new LoginResult()
            {
                Success = false,
                Message = "Email already exists."
            });
        var newUser = new ApplicationUser()
        {
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = registerRequest.Email,
            Email = registerRequest.Email,
            Name = registerRequest.Name
        };

        // setup the default role names
        string role_RegisteredUser = "RegisteredUser";
        string role_Administrator = "Administrator";

        // create the default roles (if they don't exist yet)
        if (await _roleManager.FindByNameAsync(role_RegisteredUser) == null)
        {
            await _roleManager.CreateAsync(new IdentityRole(role_RegisteredUser));
        }
        if (await _roleManager.FindByNameAsync(role_Administrator) == null)
        {
            await _roleManager.CreateAsync(new IdentityRole(role_Administrator));
        }

        // create the new user
        var isCreated = await _userManager.CreateAsync(newUser, registerRequest.Password);

        if (isCreated.Succeeded)
        {
            // the first registered user will be an admin in addition to a registered user
            if (_userManager.Users.Count() == 1)
            {
                await _userManager.AddToRoleAsync(newUser, role_Administrator);
                await _userManager.AddToRoleAsync(newUser, role_RegisteredUser);
            }
            else
            {
                await _userManager.AddToRoleAsync(newUser, role_RegisteredUser);
            }

            // confirm the e-mail and remove lockout
            newUser.EmailConfirmed = true;
            newUser.LockoutEnabled = false;

            // update changes
            await _userManager.UpdateAsync(newUser);

            var secToken = await _jwtHandler.GetTokenAsync(newUser);
            // Include the name as a claim in the JWT token
            secToken.Payload["Name"] = newUser.Name;
            var jwt = new JwtSecurityTokenHandler().WriteToken(secToken);
            return Ok(new LoginResult()
            {
                Success = true,
                Message = "Registration successful",
                Token = jwt
            });
        }
        else
        {
            return Unauthorized(new LoginResult()
            {
                Success = false,
                Message = "Registration failed"
            });
        }
    }

    // POST: api/account/update
    [HttpPost("update")]
    [ProducesResponseType(200, Type = typeof(string))]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Update(UpdateAccountRequest updateRequest)
    {
        var user = await _userManager.FindByNameAsync(updateRequest.Email);

        if (user == null)
        {
            return Unauthorized(new LoginResult()
            {
                Success = false,
                Message = "Invalid Email."
            });
        }

        // Check if the current password is correct
        var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, updateRequest.CurrentPassword);

        if (!isCurrentPasswordValid)
        {
            return Unauthorized(new LoginResult()
            {
                Success = false,
                Message = "Invalid Password."
            });
        }

        // Update the name if provided and does not match the current name
        bool isNameChanged = user.Name != updateRequest.Name;
        if (isNameChanged && !string.IsNullOrWhiteSpace(updateRequest.Name))
        {
            user.Name = updateRequest.Name;
            await _userManager.UpdateAsync(user);
        }

        // Update the password if provided
        if (!string.IsNullOrWhiteSpace(updateRequest.NewPassword))
        {
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, updateRequest.CurrentPassword, updateRequest.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                return Unauthorized(new LoginResult()
                {
                    Success = false,
                    Message = "Password update failed"
                });
            }
        }

        // Generate a new JWT token with updated user claims
        var secToken = await _jwtHandler.GetTokenAsync(user);
        // Include the name as a claim in the JWT token
        secToken.Payload["Name"] = user.Name;
        var jwt = new JwtSecurityTokenHandler().WriteToken(secToken);
        return Ok(new LoginResult()
        {
            Success = true,
            Message = "Registration successful",
            Token = jwt
        });
    }
}

