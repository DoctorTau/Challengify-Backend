using System.Text.Json.Serialization;

namespace Challengify.Entities.Models.DataTransferObject.Response;

public class ChallengeResponseDto
{
    public int ChallengeId { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime StartDate { get; set; }
    public int Periodicity { get; set; }
    public string JoinCode { get; set; }
    public List<int> ResultIds { get; set; } = [];
    public List<int> ParticipantIds { get; set; } = [];

    public ChallengeResponseDto(Challenge challenge)
    {
        ChallengeId = challenge.ChallengeId;
        Name = challenge.Name;
        Description = challenge.Description;
        StartDate = challenge.StartDate;
        Periodicity = challenge.Periodicity;
        JoinCode = challenge.JoinCode;
        ResultIds = [.. challenge.ResultsIds];
        ParticipantIds = [.. challenge.ParticipantsIds];
    }

    [JsonConstructor]
    public ChallengeResponseDto(int challengeId, string name, string description, DateTime startDate, int periodicity, string joinCode, List<int> resultIds, List<int> participantIds)
    {
        ChallengeId = challengeId;
        Name = name;
        Description = description;
        StartDate = startDate;
        Periodicity = periodicity;
        JoinCode = joinCode;
        ResultIds = resultIds;
        ParticipantIds = participantIds;
    }
}