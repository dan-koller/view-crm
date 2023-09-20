using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using View.Shared; // ApplicationUser
using View.WebApi.Data;
using View.WebApi.Repositories;

namespace View.WebApi.Controllers;

// base address: api/account
[Route(Constants.ApiRoute)]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtHandler _jwtHandler;
    private readonly IUserRepository _repository;

    public AccountController(UserManager<ApplicationUser> userManager, JwtHandler jwtHandler, IUserRepository repository)
    {
        _userManager = userManager;
        _jwtHandler = jwtHandler;
        _repository = repository;
    }

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
        var jwt = GenerateJwtToken(user).Result;
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
        var existingUser = await _userManager.FindByNameAsync(registerRequest.Email);
        if (existingUser != null)
            return Unauthorized(new LoginResult()
            {
                Success = false,
                Message = "Email already exists."
            });

        var newUser = await _repository.CreateUserAsync(registerRequest);

        if (newUser != null)
        {
            var jwt = GenerateJwtToken(newUser).Result;
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

        var updatedUser = await _repository.UpdateUserAsync(updateRequest, user);
        if (updatedUser == null)
        {
            return Unauthorized(new LoginResult()
            {
                Success = false,
                Message = "Password update failed"
            });
        }

        // Generate a new JWT token with updated claims
        var jwt = GenerateJwtToken(updatedUser).Result;
        return Ok(new LoginResult()
        {
            Success = true,
            Message = "Update successful",
            Token = jwt
        });
    }

    // Helper method to generate a JWT token. Usage: var jwt = GenerateJwtToken(user).Result;
    private async Task<String?> GenerateJwtToken(ApplicationUser user)
    {
        var secToken = await _jwtHandler.GetTokenAsync(user);
        // Include the name as a claim in the JWT token
        secToken.Payload["Name"] = user.Name;
        var jwt = new JwtSecurityTokenHandler().WriteToken(secToken);
        return jwt;
    }
}

