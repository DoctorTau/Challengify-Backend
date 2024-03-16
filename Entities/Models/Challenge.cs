using System.ComponentModel.DataAnnotations;

namespace Challengify.Models;

public enum ChallengeStatus
{
    Active,
    Inactive
}

/// <summary>
/// Represents a challenge in the Challengify application.
/// </summary>
public class Challenge
{
    [Key]
    public long ChallengeId { get; set; }
    [Required, MaxLength(256)]
    public required string Name { get; set; }
    [MaxLength(1024)]
    public string Description { get; set; } = String.Empty;
    public DateTime StartDate { get; set; } = DateTime.Now.ToUniversalTime();
    public int Periodicity { get; set; } = 24; // in hours

    // Navigation properties
    public virtual ICollection<Result> Results { get; set; } = new List<Result>();
    public virtual ICollection<User> Participants { get; set; } = new List<User>();

    public void Update(Challenge challenge)
    {
        Name = challenge.Name;
        Description = challenge.Description;
        StartDate = challenge.StartDate;
        Periodicity = challenge.Periodicity;
    }
}
