using Challengify.Entities.Database;
using Challengify.Entities.Models;
using Challengify.Entities.Models.DataTransferObject;

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
        Challenge challenge = await _dbContext.Challenges.FindAsync(challengeId) ?? throw new KeyNotFoundException("Challenge not found");
        return challenge;
    }

    public async Task<Challenge> UpdateChallengeAsync(Challenge challenge)
    {
        Challenge existingChallenge = await _dbContext.Challenges.FindAsync(challenge.ChallengeId) ?? throw new KeyNotFoundException("Challenge not found");
        existingChallenge.Update(challenge);
        await _dbContext.SaveChangesAsync();
        return existingChallenge;
    }
}