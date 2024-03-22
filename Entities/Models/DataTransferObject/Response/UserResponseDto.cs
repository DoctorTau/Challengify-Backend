namespace Challengify.Entities.Models.DataTransferObject.Response;

public class UserResponseDto
{
    public int UserId { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public List<int> ChallengeIds { get; set; } = [];
    public List<int> ResultIds { get; set; } = [];

    public UserResponseDto(User user)
    {
        UserId = user.UserId;
        Name = user.Name;
        Email = user.Email;
        CreatedAt = user.CreatedAt;
        ChallengeIds = user.Challenges.Select(c => c.ChallengeId).ToList();
        ResultIds = user.Results.Select(r => r.ResultId).ToList();
    }
}