using Challengify.Entities.Models;
using Challengify.Services;
using Microsoft.AspNetCore.Mvc;

namespace Challengify.Controllers.Endpoints;

/// <summary>
/// Represents a controller for managing user-related operations.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Retrieves a user by their ID.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve.</param>
    /// <returns>The user with the specified ID, or NotFound if the user does not exist.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _userService.GetUserAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    /// <summary>
    /// Updates a user with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the user to update.</param>
    /// <param name="user">The updated user object.</param>
    /// <returns>An IActionResult representing the result of the update operation.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(int id, User user)
    {
        if (id != user.UserId)
        {
            return BadRequest();
        }

        await _userService.UpdateUserAsync(user);

        return NoContent();
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="user">The user object to be created.</param>
    /// <returns>The created user object.</returns>
    [HttpPost]
    public async Task<ActionResult<User>> PostUser(User user)
    {
        await _userService.UpdateUserAsync(user);

        return CreatedAtAction("GetUser", new { id = user.UserId}, user);
    }

    /// <summary>
    /// Deletes a user with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the user to delete.</param>
    /// <returns>An IActionResult representing the result of the deletion operation.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var userToDelete = await _userService.DeleteUserAsync(id);

        return Ok(userToDelete);
    }
}