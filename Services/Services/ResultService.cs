using Challengify.Entities.Database;
using Challengify.Entities.Models;

namespace Challengify.Services;

public class ResultService : IResultService
{
    private readonly IAppDbContext _dbContext;

    public ResultService(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> CreateResultAsync(Result result)
    {
        await _dbContext.Results.AddAsync(result);
        await _dbContext.SaveChangesAsync();
        return result;
    }

    public async Task<Result> DeleteResultAsync(int resultId)
    {
        Result result = await _dbContext.Results.FindAsync(resultId) ?? throw new KeyNotFoundException("Result not found");
        _dbContext.Results.Remove(result);
        await _dbContext.SaveChangesAsync();
        return result;
    }

    public async Task<Result> GetResultAsync(int resultId)
    {
        Result result = await _dbContext.Results.FindAsync(resultId) ?? throw new KeyNotFoundException("Result not found");
        return result;
    }

    public async Task<Result> UpdateResultAsync(Result result)
    {
        Result existingResult = await _dbContext.Results.FindAsync(result.ResultId) ?? throw new KeyNotFoundException("Result not found");
        existingResult.Update(result);
        await _dbContext.SaveChangesAsync();
        return existingResult;
    }
}