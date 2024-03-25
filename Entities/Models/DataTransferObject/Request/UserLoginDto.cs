using System.ComponentModel.DataAnnotations;

namespace Challengify.Entities.Models.DataTransferObject
{
    public class UserLoginDto
    {
        [Required, StringLength(128)]
        public string Email { get; set; } = string.Empty;
        [Required, StringLength(128)]
        public string Password { get; set; } = string.Empty;
    }
}