using Challengify.Entities.Database;
using Challengify.Entities.Models;
using Challengify.Entities.Models.DataTransferObject;
using Challengify.Entities.Models.DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;

namespace Challengify.Services;

public class ResultService : IResultService
{
    private readonly IAppDbContext _dbContext;
    private readonly IUserService _userService;
    private readonly IChallengeService _challengeService;
    private readonly ICacheService _cacheService;

    public ResultService(IAppDbContext dbContext, IUserService userService, IChallengeService challengeService, ICacheService cacheService)
    {
        _dbContext = dbContext;
        _userService = userService;
        _challengeService = challengeService;
        _cacheService = cacheService;
    }

    public async Task<Result> CreateResultAsync(ResultCreationDto result)
    {
        User user = await _userService.GetUserAsync(result.UserId);
        Challenge challenge = await _challengeService.GetChallengeAsync(result.ChallengeId);

        Result newResult = new()
        {
            Name = result.Name,
            Description = result.Description,
            MediaPath = result.MediaPath,
            UserId = user.UserId,
            ChallengeId = challenge.ChallengeId,
            Timestamp = DateTime.Now.ToUniversalTime()
        };

        Console.WriteLine(newResult);

        await _dbContext.Results.AddAsync(newResult);
        await _dbContext.SaveChangesAsync();
        await _cacheService.RemoveAsync($"challenge_{newResult.ChallengeId}_user_{newResult.UserId}_last_result");

        return newResult;
    }

    public async Task<Result> DeleteResultAsync(int resultId)
    {
        Result result = await _dbContext.Results.FindAsync(resultId) ?? throw new KeyNotFoundException("Result not found");
        _dbContext.Results.Remove(result);
        await _cacheService.RemoveAsync($"challenge_{result.ChallengeId}_user_{result.UserId}_last_result");
        await _dbContext.SaveChangesAsync();
        return result;
    }

    public async Task<ResultResponseDto> GetResultAsync(int resultId)
    {
        Result result = await _dbContext.Results.FirstOrDefaultAsync(r => r.ResultId == resultId)
                        ?? throw new KeyNotFoundException("Result not found");

        return new ResultResponseDto(result);
    }

    public async Task<Result> UpdateResultAsync(Result result)
    {
        Result existingResult = await _dbContext.Results.FindAsync(result.ResultId) ?? throw new KeyNotFoundException("Result not found");
        existingResult.Update(result);
        await _cacheService.RemoveAsync($"challenge_{existingResult.ChallengeId}_user_{existingResult.UserId}_last_result");
        await _dbContext.SaveChangesAsync();
        return existingResult;
    }

    public async Task<List<ResultResponseDto>> GetResultsByChallengeIdAsync(int challengeId)
    {
        List<Result> results = await _dbContext.Results.Where(r => r.ChallengeId == challengeId)
                                                       .ToListAsync();
        List<ResultResponseDto> resultResponseDtos = results.Select(r => new ResultResponseDto(r)).ToList();
        return resultResponseDtos;
    }

    public async Task<List<ResultResponseDto>> GetResultsByUserIdAsync(int userId)
    {
        List<Result> results = await _dbContext.Results
            .Where(r => r.UserId == userId)
            .ToListAsync();
        List<ResultResponseDto> resultResponseDtos = results.Select(r => new ResultResponseDto(r)).ToList();
        return resultResponseDtos;
    }

    public async Task<ResultResponseDto?> GetLastResultByChallengeIdAsync(int challengeId, int userId)
    {
        string cacheKey = $"challenge_{challengeId}_user_{userId}_last_result";
        ResultResponseDto? lastResult = await _cacheService.GetObjectAsync<ResultResponseDto>(cacheKey);

        if (lastResult == null)
        {
            // Find the last result for the specified challenge and user. If no result is found, return null.
            Result? result = await _dbContext.Results.Where(r => r.ChallengeId == challengeId && r.UserId == userId)
                                                    .OrderByDescending(r => r.Timestamp)
                                                    .FirstOrDefaultAsync();
            if (result == null)
            {
                return null;
            }

            lastResult = new ResultResponseDto(result);
            await _cacheService.SetObjectAsync(cacheKey, lastResult, TimeSpan.FromMinutes(5));
        }



        return lastResult;
    }
}