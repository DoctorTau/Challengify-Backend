using Challengify.Entities.Database;
using Challengify.Entities.Models;

namespace Challengify.Services;

public class ChallengeService : IChallengeService
{
    private readonly IAppDbContext _dbContext;

    public ChallengeService(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Challenge> CreateChallengeAsync(Challenge challenge)
    {
        await _dbContext.Challenges.AddAsync(challenge);
        await _dbContext.SaveChangesAsync();
        return challenge;
    }

    public async Task<Challenge> DeleteChallengeAsync(long challengeId)
    {
        Challenge challenge = await _dbContext.Challenges.FindAsync(challengeId) ?? throw new KeyNotFoundException("Challenge not found");
        _dbContext.Challenges.Remove(challenge);
        await _dbContext.SaveChangesAsync();
        return challenge;
    }

    public async Task<Challenge> GetChallengeAsync(long challengeId)
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