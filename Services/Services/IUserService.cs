using Challengify.Entities.Models;

namespace Challengify.Services;

/// <summary>
/// Represents a service for managing user-related operations.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="user">The user object containing the user details.</param>
    /// <returns>The created user object.</returns>
    public Task<User> CreateUserAsync(User user);

    /// <summary>
    /// Retrieves a user by their ID.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve.</param>
    /// <returns>The user object corresponding to the specified ID.</returns>
    public Task<User> GetUserAsync(long userId);

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="user">The user object containing the updated user details.</param>
    /// <returns>The updated user object.</returns>
    public Task<User> UpdateUserAsync(User user);

    /// <summary>
    /// Deletes a user by their ID.
    /// </summary>
    /// <param name="userId">The ID of the user to delete.</param>
    /// <returns>The deleted user object.</returns>
    public Task<User> DeleteUserAsync(long userId);
}