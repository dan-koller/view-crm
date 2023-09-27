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
            return Unauthorized(CreateLoginResult(false, "Invalid Email or Password."));
        var jwt = GenerateJwtToken(user).Result;
        return Ok(CreateLoginResult(true, "Login successful", jwt));
    }

    // POST: api/account/register
    [HttpPost("register")]
    [ProducesResponseType(200, Type = typeof(string))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Register(RegisterRequest registerRequest)
    {
        var existingUser = await _userManager.FindByNameAsync(registerRequest.Email);
        if (existingUser != null)
            return Unauthorized(CreateLoginResult(false, "Email already exists."));

        var newUser = await _repository.CreateUserAsync(registerRequest);
        if (newUser == null)
            return Unauthorized(CreateLoginResult(false, "Registration failed."));

        var jwt = GenerateJwtToken(newUser).Result;
        return Ok(CreateLoginResult(true, "Registration successful", jwt));
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
        if (user == null || !await _userManager.CheckPasswordAsync(user, updateRequest.CurrentPassword))
            return Unauthorized(CreateLoginResult(false, "Invalid Email or Password."));

        var updatedUser = await _repository.UpdateUserAsync(updateRequest, user);
        if (updatedUser == null)
            return Unauthorized(CreateLoginResult(false, "Account update failed."));

        var jwt = GenerateJwtToken(updatedUser).Result;
        return Ok(CreateLoginResult(true, "Update successful", jwt));
    }

    // Helper method to generate a JWT token including the name as a claim
    // Usage: var jwt = GenerateJwtToken(user).Result;
    private async Task<String?> GenerateJwtToken(ApplicationUser user)
    {
        var secToken = await _jwtHandler.GetTokenAsync(user);
        secToken.Payload["Name"] = user.Name;
        var jwt = new JwtSecurityTokenHandler().WriteToken(secToken);
        return jwt;
    }

    private LoginResult CreateLoginResult(bool success, string message, string? token = null)
    {
        return new LoginResult()
        {
            Success = success,
            Message = message,
            Token = token
        };
    }
}

