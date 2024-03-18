using Challengify.Entities.Models;
using Challengify.Entities.Models.DataTransferObject;
using Challengify.Services;
using Microsoft.AspNetCore.Mvc;

namespace Challengify.Controllers.Endpoints;

/// <summary>
/// Controller for handling authentication-related endpoints.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController(IUserService userService, IPasswordService passwordService) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly IPasswordService _passwordService = passwordService;

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="userRegistrationDto">The user registration data.</param>
    /// <returns>The newly created user.</returns>
    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(UserRegistrationDto userRegistrationDto)
    {
        _passwordService.CreatePasswordHash(userRegistrationDto.Password, out string passwordHash, out string passwordSalt);

        var user = new User
        {
            Name = userRegistrationDto.Username,
            Email = userRegistrationDto.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        await _userService.CreateUserAsync(user);

        return CreatedAtAction("GetUser", new { controller = "User", id = user.UserId }, user);
    }

    /// <summary>
    /// Logs in a user with the provided credentials.
    /// </summary>
    /// <param name="userLoginDto">The user login data transfer object.</param>
    /// <returns>An action result containing the logged-in user if successful, or an appropriate error response.</returns>
    [HttpPost("login")]
    public async Task<ActionResult<User>> Login(UserLoginDto userLoginDto)
    {
        try
        {
            var user = await _userService.GetUserByEmailAsync(userLoginDto.Email);
            if (!_passwordService.VerifyPasswordHash(userLoginDto.Password, user.PasswordHash, user.PasswordSalt))
            {
                return Unauthorized();
            }
            return Ok(_passwordService.CreateJwtToken(user));
        }
        catch (KeyNotFoundException)
        {
            return NotFound("User not found");
        }
    }
}


