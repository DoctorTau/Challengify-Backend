namespace Services.Utils;

public interface IChallengeCodeGenerator
{
    public string GenerateJoinCode(int challengeId, DateTime creationDate);
}
