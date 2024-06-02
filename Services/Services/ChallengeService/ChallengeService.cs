using Challengify.Entities.Database;
using Challengify.Entities.Models;
using Challengify.Entities.Models.DataTransferObject;
using Challengify.Entities.Models.DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using Services.Utils;

namespace Challengify.Services;

public class ChallengeService : IChallengeService
{
    private readonly IAppDbContext _dbContext;
    private readonly IUserService _userService;
    private readonly IChallengeCodeGenerator _challengeCodeGenerator;
    private readonly ICacheService _cacheService;

    public ChallengeService(IAppDbContext dbContext, IUserService userService, IChallengeCodeGenerator challengeCodeGenerator, ICacheService cacheService)
    {
        _dbContext = dbContext;
        _userService = userService;
        _challengeCodeGenerator = challengeCodeGenerator;
        _cacheService = cacheService;
    }

    public async Task<Challenge> CreateChallengeAsync(Challenge challenge)
    {
        challenge.JoinCode = await CreateJoinCode(challenge);
        await _dbContext.Challenges.AddAsync(challenge);
        await _dbContext.SaveChangesAsync();
        return challenge;
    }

    public async Task<Challenge> CreateChallengeAsync(ChallengeCreationDto challenge, int userId)
    {
        User user = await _userService.GetUserAsync(userId);
        Challenge newChallenge = new()
        {
            Name = challenge.Title,
            Description = challenge.Description,
            StartDate = DateTime.Now.ToUniversalTime(),
            Periodicity = challenge.Periodicity,
            ParticipantsIds = [user.UserId]
        };

        return await CreateChallengeAsync(newChallenge);
    }

    public async Task<Challenge> DeleteChallengeAsync(int challengeId)
    {
        Challenge challenge = await _dbContext.Challenges.FindAsync(challengeId) ?? throw new KeyNotFoundException("Challenge not found");
        _dbContext.Challenges.Remove(challenge);
        await _dbContext.SaveChangesAsync();
        return challenge;
    }

    public async Task<Challenge> GetChallengeAsync(int challengeId)
    {
        string cacheKey =  $"challenge_{challengeId}";
        Challenge? challenge = await _cacheService.GetObjectAsync<Challenge>(cacheKey);
        if(challenge == null)
        {
            challenge = await _dbContext.Challenges .FirstOrDefaultAsync(c => c.ChallengeId == challengeId) ?? throw new KeyNotFoundException("Challenge not found");
            await _cacheService.SetObjectAsync(cacheKey, challenge, TimeSpan.FromMinutes(5));
        }

        return challenge;
    }

    public async Task<ChallengeResponseDto> GetChallengeResponseDtoAsync(int challengeId)
    {
        Challenge challenge = await GetChallengeAsync(challengeId);
        return new ChallengeResponseDto(challenge);
    }

    public async Task<List<ChallengeResponseDto>> GetUserChallengesAsync(int userId)
    {
        string cacheKey = $"user_{userId}_challenges";
        List<ChallengeResponseDto>? userChallenges = await _cacheService.GetObjectAsync<List<ChallengeResponseDto>>(cacheKey);
        if(userChallenges == null)
        {
            userChallenges = (await _dbContext.Challenges.Where(c => c.ParticipantsIds.Any(p => p == userId)).ToListAsync()).Select(c => new ChallengeResponseDto(c)).ToList();

            await _cacheService.SetObjectAsync(cacheKey, userChallenges, TimeSpan.FromMinutes(5));
        }

        return userChallenges;
    }

    public async Task<Challenge> UpdateChallengeAsync(Challenge challenge)
    {
        Challenge existingChallenge = await _dbContext.Challenges.FindAsync(challenge.ChallengeId) ?? throw new KeyNotFoundException("Challenge not found");
        existingChallenge.Update(challenge);
        await ClearCache(challenge.ChallengeId);
        await _dbContext.SaveChangesAsync();
        return existingChallenge;
    }

    public async Task<ChallengeResponseDto> AddParticipantAsync(int challengeId, int userId)
    {
        Challenge challenge = await _dbContext.Challenges.FirstOrDefaultAsync(c => c.ChallengeId == challengeId)
                              ?? throw new KeyNotFoundException("Challenge not found");
        challenge.ParticipantsIds.Add(userId);
        await ClearCache(challengeId);
        await _dbContext.SaveChangesAsync();
        return new ChallengeResponseDto(challenge);
    }

    public async Task<ChallengeResponseDto> JoinChallengeAsync(string joinCode, int userId)
    {
        Console.WriteLine(joinCode);
        Challenge challenge = await _dbContext.Challenges.FirstOrDefaultAsync(c => c.JoinCode == joinCode)
            ?? throw new KeyNotFoundException("Challenge not found");

        Console.WriteLine(challenge.ChallengeId);

        return await AddParticipantAsync(challenge.ChallengeId, userId);
    }

    public async Task<List<ResultResponseDto>> GetUserResultsAsync(int userId)
    {
        string cacheKey = $"user_{userId}_results";
        List<ResultResponseDto>? userResults = await _cacheService.GetObjectAsync<List<ResultResponseDto>>(cacheKey);

        if(userResults == null)
        {
            userResults = (await _dbContext.Results.Where(r => r.UserId == userId)
                                                   .ToListAsync()).Select(r => new ResultResponseDto(r)).ToList();
            await _cacheService.SetObjectAsync(cacheKey, userResults, TimeSpan.FromMinutes(5));
        }

        return userResults;
    }

    public async Task<List<ResultResponseDto>> GetChallengeResultsAsync(int challengeId)
    {
        Challenge challenge = await GetChallengeAsync(challengeId);
        List<Result> challengeResults = await _dbContext.Results.Where(r => r.ChallengeId == challenge.ChallengeId)
                                                               .ToListAsync();
        List<ResultResponseDto> resultResponseDtos = challengeResults.Select(r => new ResultResponseDto(r)).ToList();
        return resultResponseDtos;
    }

    private async Task<string> CreateJoinCode(Challenge challenge)
    {
        do
        {
            var joinCode = _challengeCodeGenerator.GenerateJoinCode(challenge.ChallengeId, challenge.StartDate);
            if (!await _dbContext.Challenges.AnyAsync(c => c.JoinCode == joinCode))
            {
                return joinCode;
            }
        } while (true);
    }

    private async Task ClearCache(int challengeId)
    {
        await _cacheService.RemoveAsync($"challenge_{challengeId}");
        await _cacheService.RemoveAsync($"user_{challengeId}_challenges");
        await _cacheService.RemoveAsync($"user_{challengeId}_results");
    }
}
