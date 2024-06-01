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

    public ResultService(IAppDbContext dbContext, IUserService userService, IChallengeService challengeService)
    {
        _dbContext = dbContext;
        _userService = userService;
        _challengeService = challengeService;
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
            User = user,
            Challenge = challenge,
            Timestamp = DateTime.Now.ToUniversalTime()
        };

        Console.WriteLine(newResult);

        await _dbContext.Results.AddAsync(newResult);
        await _dbContext.SaveChangesAsync();

        return newResult;
    }

    public async Task<Result> DeleteResultAsync(int resultId)
    {
        Result result = await _dbContext.Results.FindAsync(resultId) ?? throw new KeyNotFoundException("Result not found");
        _dbContext.Results.Remove(result);
        await _dbContext.SaveChangesAsync();
        return result;
    }

    public async Task<ResultResponseDto> GetResultAsync(int resultId)
    {
        Result result = await _dbContext.Results.Include(r => r.User)
                                                .Include(r => r.Challenge)
                                                .FirstOrDefaultAsync(r => r.ResultId == resultId) ?? throw new KeyNotFoundException("Result not found");

        return new ResultResponseDto(result);
    }

    public async Task<Result> UpdateResultAsync(Result result)
    {
        Result existingResult = await _dbContext.Results.FindAsync(result.ResultId) ?? throw new KeyNotFoundException("Result not found");
        existingResult.Update(result);
        await _dbContext.SaveChangesAsync();
        return existingResult;
    }

    public async Task<List<ResultResponseDto>> GetResultsByChallengeIdAsync(int challengeId)
    {
        List<Result> results = await _dbContext.Results.Include(r => r.User)
                                                       .Include(r => r.Challenge)
                                                       .Where(r => r.Challenge.ChallengeId == challengeId)
                                                       .ToListAsync();
        List<ResultResponseDto> resultResponseDtos = results.Select(r => new ResultResponseDto(r)).ToList();
        return resultResponseDtos;
    }

    public async Task<List<ResultResponseDto>> GetResultsByUserIdAsync(int userId)
    {
        List<Result> results = await _dbContext.Results
            .Where(r => r.User.UserId == userId)
            .ToListAsync();
        List<ResultResponseDto> resultResponseDtos = results.Select(r => new ResultResponseDto(r)).ToList();
        return resultResponseDtos;
    }

    public async Task<ResultResponseDto?> GetLastResultByChallengeIdAsync(int challengeId, int userId)
    {
        // Find the last result for the specified challenge and user. If no result is found, return null.
        Result? result = await _dbContext.Results.Include(r => r.User)
                                                .Include(r => r.Challenge)
                                                .Where(r => r.Challenge.ChallengeId == challengeId && r.User.UserId == userId)
                                                .OrderByDescending(r => r.Timestamp)
                                                .FirstOrDefaultAsync();

        return result == null ? null : new ResultResponseDto(result);
    }
}