using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Challengify.Models;

/// <summary>
/// Represents a result of a challenge.
/// </summary>
public class Result
{
    [Key]
    public long ResultId { get; set; }
    [Required]
    public long UserId { get; set; }
    [Required]
    public long ChallengeId { get; set; }
    [Required, MaxLength(128)]
    public required string Name { get; set; }
    [MaxLength(512)]
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now.ToUniversalTime();
    public string MediaPath { get; set; } = string.Empty;

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual required User User { get; set; }
    [ForeignKey("ChallengeId")]
    public virtual required Challenge Challenge { get; set; }

    public void Update(Result result)
    {
        UserId = result.UserId;
        ChallengeId = result.ChallengeId;
        Name = result.Name;
        Description = result.Description;
        Timestamp = result.Timestamp;
        MediaPath = result.MediaPath;
    }
}
