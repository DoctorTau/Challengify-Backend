using System.ComponentModel.DataAnnotations;

namespace Challengify.Entities.Models;

public enum UserStatus
{
    Regular,
    Premium
}

/// <summary>
/// Represents a user in the system.
/// </summary>
public class User
{
    [Key]
    public int UserId { get; set; }
    [Required, MaxLength(64)]
    public required string Name { get; set; }
    [Required, EmailAddress, MaxLength(128)]
    public required string Email { get; set; }
    [Required]
    public required string PasswordHash { get; set; }
    public required DateTime CreatedAt { get; set; } = DateTime.Now.ToUniversalTime();
    public required UserStatus Status { get; set; } = UserStatus.Regular;

    // Navigation properties
    public virtual ICollection<Challenge> Challenges { get; set; } = new List<Challenge>();
    public virtual ICollection<Result> Results { get; set; } = new List<Result>();

    public void Update(User user)
    {
        Name = user.Name;
        Email = user.Email;
        PasswordHash = user.PasswordHash;
        CreatedAt = user.CreatedAt;
        Status = user.Status;
    }
}

