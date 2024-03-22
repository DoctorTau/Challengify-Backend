using Challengify.Entities.Database;
using Challengify.Entities.Models;
using Challengify.Entities.Models.DataTransferObject;
using Challengify.Entities.Models.DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;

namespace Challengify.Services;

public class ChallengeService : IChallengeService
{
    private readonly IAppDbContext _dbContext;
    private readonly IUserService _userService;

    public ChallengeService(IAppDbContext dbContext, IUserService userService)
    {
        _dbContext = dbContext;
        _userService = userService;
    }

    public async Task<Challenge> CreateChallengeAsync(Challenge challenge)
    {
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
            Participants = [user]
        };

        await _dbContext.Challenges.AddAsync(newChallenge);
        await _dbContext.SaveChangesAsync();

        return newChallenge;
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
        Challenge challenge = await _dbContext.Challenges.Include(c => c.Participants)
                                                         .Include(c => c.Results)
                                                         .FirstOrDefaultAsync(c => c.ChallengeId == challengeId) ?? throw new KeyNotFoundException("Challenge not found");

        return challenge;
    }

    public async Task<ChallengeResponseDto> GetChallengeResponseDtoAsync(int challengeId)
    {
        Challenge challenge = await GetChallengeAsync(challengeId);
        return new ChallengeResponseDto(challenge);
    }

    public async Task<List<ChallengeResponseDto>> GetUserChallengesAsync(int userId)
    {
        List<Challenge> userChallenges = await _dbContext.Challenges.Include(c => c.Participants)
                                                                    .Include(c => c.Results)
                                                                    .Where(c => c.Participants.Any(p => p.UserId == userId))
                                                                    .ToListAsync();

        List<ChallengeResponseDto> challengeResponseDtos = userChallenges.Select(c => new ChallengeResponseDto(c)).ToList();
        return challengeResponseDtos;
    }

    public async Task<Challenge> UpdateChallengeAsync(Challenge challenge)
    {
        Challenge existingChallenge = await _dbContext.Challenges.FindAsync(challenge.ChallengeId) ?? throw new KeyNotFoundException("Challenge not found");
        existingChallenge.Update(challenge);
        await _dbContext.SaveChangesAsync();
        return existingChallenge;
    }

    public async Task<ChallengeResponseDto> AddParticipantAsync(int challengeId, int userId)
    {
        Challenge challenge = await _dbContext.Challenges.Include(c => c.Participants).FirstOrDefaultAsync(c => c.ChallengeId == challengeId) ?? throw new KeyNotFoundException("Challenge not found");
        User user = await _userService.GetUserAsync(userId);
        challenge.Participants.Add(user);
        await _dbContext.SaveChangesAsync();
        return new ChallengeResponseDto(challenge);
    }

    public async Task<List<ResultResponseDto>> GetUserResultsAsync(int userId)
    {
        List<Result> userResults = await _dbContext.Results.Include(r => r.User)
                                                           .Include(r => r.Challenge)
                                                           .Where(r => r.User.UserId == userId)
                                                           .ToListAsync();
        List<ResultResponseDto> resultResponseDtos = userResults.Select(r => new ResultResponseDto(r)).ToList();
        return resultResponseDtos;
    }

    public async Task<List<ResultResponseDto>> GetChallengeResultsAsync(int challengeId)
    {
        Challenge challenge = await GetChallengeAsync(challengeId);
        List<Result> challengeResults = (List<Result>)challenge.Results;
        List<ResultResponseDto> resultResponseDtos = challengeResults.Select(r => new ResultResponseDto(r)).ToList();
        return resultResponseDtos;
    }

}