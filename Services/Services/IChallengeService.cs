using Challengify.Entities.Models;

namespace Challengify.Services;

/// <summary>
/// Represents a service for managing challenges.
/// </summary>
public interface IChallengeService
{
    /// <summary>
    /// Creates a new challenge.
    /// </summary>
    /// <param name="challenge">The challenge to create.</param>
    /// <returns>The created challenge.</returns>
    public Task<Challenge> CreateChallengeAsync(Challenge challenge);

    /// <summary>
    /// Retrieves a challenge by its ID.
    /// </summary>
    /// <param name="challengeId">The ID of the challenge to retrieve.</param>
    /// <returns>The retrieved challenge.</returns>
    public Task<Challenge> GetChallengeAsync(int challengeId);

    /// <summary>
    /// Updates an existing challenge.
    /// </summary>
    /// <param name="challenge">The challenge to update.</param>
    /// <returns>The updated challenge.</returns>
    public Task<Challenge> UpdateChallengeAsync(Challenge challenge);

    /// <summary>
    /// Deletes a challenge by its ID.
    /// </summary>
    /// <param name="challengeId">The ID of the challenge to delete.</param>
    /// <returns>The deleted challenge.</returns>
    public Task<Challenge> DeleteChallengeAsync(int challengeId);
}