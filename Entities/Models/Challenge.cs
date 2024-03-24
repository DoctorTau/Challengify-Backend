using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Challengify.Entities.Models;

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
    public int ChallengeId { get; set; }
    [Required, MaxLength(256)]
    public required string Name { get; set; }
    [MaxLength(1024)]
    public string Description { get; set; } = String.Empty;
    public DateTime StartDate { get; set; } = DateTime.Now.ToUniversalTime();
    public int Periodicity { get; set; } = 24; // in hours
    public string JoinCode { get; set; } = Guid.NewGuid().ToString();

    // Navigation properties
    [InverseProperty("Challenge")]
    public virtual ICollection<Result> Results { get; set; } = [];
    [InverseProperty("Challenges")]
    public virtual ICollection<User> Participants { get; set; } = [];

    public void Update(Challenge challenge)
    {
        Name = challenge.Name;
        Description = challenge.Description;
        StartDate = challenge.StartDate;
        Periodicity = challenge.Periodicity;
    }
}
