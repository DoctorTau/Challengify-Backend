using System.ComponentModel.DataAnnotations;

namespace Challengify.Entities.Models.DataTransferObject;

public class ResultCreateRequestDto
{
    [Required, MaxLength(128)]
    public string Name { get; set; } = string.Empty;
    [Required, MaxLength(512)]
    public string Description { get; set; } = string.Empty;
    public string MediaPath { get; set; } = string.Empty;
}

public class ResultCreationDto
{
    public int UserId { get; set; }
    public int ChallengeId { get; set; }
    [Required, MaxLength(128)]
    public string Name { get; set; } = string.Empty;
    [Required, MaxLength(512)]
    public string Description { get; set; } = string.Empty;
    public string MediaPath { get; set; } = string.Empty;
}