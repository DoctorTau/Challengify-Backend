using Challengify.Entities.Models;
using Challengify.Entities.Models.DataTransferObject;
using Challengify.Entities.Models.DataTransferObject.Response;

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
    /// Retrieves the ChallengeResponseDto asynchronously for the specified challenge ID.
    /// </summary>
    /// <param name="challengeId">The ID of the challenge.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the ChallengeResponseDto.</returns>
    public Task<ChallengeResponseDto> GetChallengeResponseDtoAsync(int challengeId);

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

    /// <summary>
    /// Creates a new challenge asynchronously.
    /// </summary>
    /// <param name="challenge"> Challenge creation params </param>
    /// <param name="userId"> Creator</param>
    /// <returns>Created challenge</returns>
    public Task<Challenge> CreateChallengeAsync(ChallengeCreationDto challenge, int userId);

    /// <summary>
    /// Retrieves the challenges associated with a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of challenges.</returns>
    public Task<List<ChallengeResponseDto>> GetUserChallengesAsync(int userId);

    /// <summary>
    /// Adds a participant to a challenge asynchronously.
    /// </summary>
    /// <param name="challengeId">The ID of the challenge.</param>
    /// <param name="userId">The ID of the user to add as a participant.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the updated Challenge object.</returns>
    public Task<ChallengeResponseDto> AddParticipantAsync(int challengeId, int userId);

    /// <summary>
    /// Retrieves the results of a challenge asynchronously.
    /// </summary>
    /// <param name="challengeId">The ID of the challenge.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of results.</returns>
    public Task<List<ResultResponseDto>> GetChallengeResultsAsync(int challengeId);

    /// <summary>
    /// Retrieves the results for a specific user asynchronously.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="Result"/>.</returns>
    public Task<List<ResultResponseDto>> GetUserResultsAsync(int userId);
}