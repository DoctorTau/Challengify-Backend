using Challengify.Entities.Models;
using Challengify.Entities.Models.DataTransferObject;
using Challengify.Services;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IPasswordService _passwordService;

    public AuthController(IUserService userService, IPasswordService passwordService)
    {
        _userService = userService;
        _passwordService = passwordService;
    }

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


