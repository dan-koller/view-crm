using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using View.WebApi.Data;
using View.Shared; // ApplicationUser

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
    public async Task<IActionResult> Register(LoginRequest loginRequest)
    {
        var user = await _userManager.FindByNameAsync(loginRequest.Email);
        if (user != null)
            return Unauthorized(new LoginResult()
            {
                Success = false,
                Message = "Email already exists."
            });
        var newUser = new ApplicationUser()
        {
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = loginRequest.Email,
            Email = loginRequest.Email,
        };

        // create the new user
        var isCreated = await _userManager.CreateAsync(newUser, loginRequest.Password);

        if (isCreated.Succeeded)
        {
            // the first registered user will be an admin in addition to a registered user
            if (_userManager.Users.Count() == 0)
            {
                await _userManager.AddToRolesAsync(newUser, new List<string>() { "Administrator", "RegisteredUser" });
            }
            else
            {
                await _userManager.AddToRoleAsync(newUser, "RegisteredUser");
            }

            // confirm the e-mail and remove lockout
            newUser.EmailConfirmed = true;
            newUser.LockoutEnabled = false;

            // update changes
            await _userManager.UpdateAsync(newUser);

            var secToken = await _jwtHandler.GetTokenAsync(newUser);
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
}

