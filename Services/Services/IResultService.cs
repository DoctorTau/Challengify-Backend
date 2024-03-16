using Challengify.Models;

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
    public Task<Result> CreateResultAsync(Result result);

    /// <summary>
    /// Retrieves a result by its ID asynchronously.
    /// </summary>
    /// <param name="resultId">The ID of the result to retrieve.</param>
    /// <returns>The retrieved result.</returns>
    public Task<Result> GetResultAsync(long resultId);

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
    public Task<Result> DeleteResultAsync(long resultId);
}