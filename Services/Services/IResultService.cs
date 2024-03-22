using Challengify.Entities.Models;
using Challengify.Entities.Models.DataTransferObject;
using Challengify.Entities.Models.DataTransferObject.Response;

namespace Challengify.Services;

/// <summary>
/// Represents a service for managing results.
/// </summary>
public interface IResultService
{
    /// <summary>
    /// Creates a new result asynchronously.
    /// </summary>
    /// <param name="result">The result to create.</param>
    /// <returns>The created result.</returns>
    public Task<Result> CreateResultAsync(ResultCreationDto result);

    /// <summary>
    /// Retrieves a result by its ID asynchronously.
    /// </summary>
    /// <param name="resultId">The ID of the result to retrieve.</param>
    /// <returns>The retrieved result.</returns>
    public Task<ResultResponseDto> GetResultAsync(int resultId);

    /// <summary>
    /// Retrieves a list of results by challenge ID asynchronously.
    /// </summary>
    /// <param name="challengeId">The ID of the challenge.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of results.</returns>
    public Task<List<ResultResponseDto>> GetResultsByChallengeIdAsync(int challengeId);

    /// <summary>
    /// Retrieves a list of results for a given user ID asynchronously.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of results.</returns>
    public Task<List<ResultResponseDto>> GetResultsByUserIdAsync(int userId);

    /// <summary>
    /// Updates an existing result asynchronously.
    /// </summary>
    /// <param name="result">The result to update.</param>
    /// <returns>The updated result.</returns>
    public Task<Result> UpdateResultAsync(Result result);

    /// <summary>
    /// Deletes a result by its ID asynchronously.
    /// </summary>
    /// <param name="resultId">The ID of the result to delete.</param>
    /// <returns>The deleted result.</returns>
    public Task<Result> DeleteResultAsync(int resultId);
}