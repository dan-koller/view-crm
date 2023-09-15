using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using View.WebApi.Data;
using View.Shared; // ApplicationUser
using Microsoft.AspNetCore.Authorization;
using View.WebApi.Repositories;

namespace View.WebApi.Controllers;

// base address: api/account
[Route(Constants.ApiRoute)]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtHandler _jwtHandler;

    private readonly IUserRepository _repository;

    public AccountController(UserContext context, UserManager<ApplicationUser> userManager, JwtHandler jwtHandler, IUserRepository repository)
    {
        _context = context;
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

        // Generate a new JWT token with updated existingUser claims
        var secToken = await _jwtHandler.GetTokenAsync(user);
        // Include the name as a claim in the JWT token
        secToken.Payload["Name"] = user.Name;
        var jwt = new JwtSecurityTokenHandler().WriteToken(secToken);
        return Ok(new LoginResult()
        {
            Success = true,
            Message = "Update successful",
            Token = jwt
        });
    }
}

