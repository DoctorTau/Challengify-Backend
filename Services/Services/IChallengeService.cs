using Challengify.Models;

namespace Challengify.Services;

public interface IChallengeService
{
    public Task<Challenge> CreateChallengeAsync(Challenge challenge);
    public Task<Challenge> GetChallengeAsync(long challengeId);
    public Task<Challenge> UpdateChallengeAsync(Challenge challenge);
    public Task<Challenge> DeleteChallengeAsync(long challengeId);
}