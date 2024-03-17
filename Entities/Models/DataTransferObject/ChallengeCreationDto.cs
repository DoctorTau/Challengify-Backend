using System.ComponentModel.DataAnnotations;

namespace Challengify.Entities.Models.DataTransferObject
{
    public class ChallengeCreationDto
    {
        [Required, StringLength(256)]
        public string Title { get; set; } = string.Empty;
        [StringLength(1024)]
        public string Description { get; set; } = string.Empty;
        public int Periodicity { get; set; } = 24; // in hours
    }
}