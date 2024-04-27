using System.ComponentModel.DataAnnotations;

namespace Zeiterfassung.DTO;

public class WorkSessionDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public DateTime Start { get; set; }

    public DateTime? End { get; set; }

    [Required]
    public int LocationId { get; set; }

    [Required]
    public int ProjectId { get; set; }
}