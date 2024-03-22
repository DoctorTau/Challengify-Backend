namespace Challengify.Entities.Models.DataTransferObject.Response;

public class ResultResponseDto
{
    public int ResultId { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime Timestamp { get; set; }
    public string MediaPath { get; set; } = "";
    public int UserId { get; set; }
    public int ChallengeId { get; set; }

    public ResultResponseDto(Result result)
    {
        ResultId = result.ResultId;
        Name = result.Name;
        Description = result.Description;
        Timestamp = result.Timestamp;
        MediaPath = result.MediaPath;
        UserId = result.User.UserId;
        ChallengeId = result.Challenge.ChallengeId;
    }
}