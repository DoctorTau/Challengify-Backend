using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    [Required]
    public required string PasswordSalt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now.ToUniversalTime();
    public UserStatus Status { get; set; } = UserStatus.Regular;

    public virtual IList<int> ChallengesIds { get; set; } = [];
    public virtual IList<int> ResultsIds { get; set; } = [];

    public void Update(User user)
    {
        Name = user.Name;
        Email = user.Email;
        PasswordHash = user.PasswordHash;
        CreatedAt = user.CreatedAt;
        Status = user.Status;
    }
}

