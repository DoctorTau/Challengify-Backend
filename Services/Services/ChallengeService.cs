using Challengify.Entities.Database;
using Challengify.Entities.Models;
using Challengify.Entities.Models.DataTransferObject;
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
        Challenge challenge = await _dbContext.Challenges.FindAsync(challengeId) ?? throw new KeyNotFoundException("Challenge not found");
        return challenge;
    }

    public async Task<List<Challenge>> GetUserChallengesAsync(int userId)
    {
        List<Challenge> userChallenges = await _dbContext.Challenges.Where(c => c.Participants.Any(p => p.UserId == userId)).ToListAsync();
        return userChallenges;
    }

    public async Task<Challenge> UpdateChallengeAsync(Challenge challenge)
    {
        Challenge existingChallenge = await _dbContext.Challenges.FindAsync(challenge.ChallengeId) ?? throw new KeyNotFoundException("Challenge not found");
        existingChallenge.Update(challenge);
        await _dbContext.SaveChangesAsync();
        return existingChallenge;
    }

    public async Task<Challenge> AddParticipantAsync(int challengeId, int userId)
    {
        Challenge challenge = await _dbContext.Challenges.Include(c => c.Participants).FirstOrDefaultAsync(c => c.ChallengeId == challengeId) ?? throw new KeyNotFoundException("Challenge not found");
        User user = await _userService.GetUserAsync(userId);
        challenge.Participants.Add(user);
        await _dbContext.SaveChangesAsync();
        return challenge;
    }
}