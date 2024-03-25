using System.ComponentModel.DataAnnotations;

namespace Challengify.Entities.Models.DataTransferObject;

public class UserRegistrationDto
{
    [Required, StringLength(64)]
    public string Username { get; set; } = string.Empty;
    [Required, EmailAddress, StringLength(128)]
    public string Email { get; set; } = string.Empty;
    [Required, StringLength(128)]
    public string Password { get; set; } = string.Empty;
    [Required, Compare("Password")]
    public string PasswordConfirmation { get; set; } = string.Empty;
}