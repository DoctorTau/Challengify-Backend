using Challengify.Models;

namespace Challengify.Services;

public interface IResultService
{
    public Task<Result> CreateResultAsync(Result result);
    public Task<Result> GetResultAsync(long resultId);
    public Task<Result> UpdateResultAsync(Result result);
    public Task<Result> DeleteResultAsync(long resultId);
}