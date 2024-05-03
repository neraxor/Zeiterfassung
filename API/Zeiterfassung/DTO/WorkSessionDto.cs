using System.ComponentModel.DataAnnotations;

namespace Zeiterfassung.DTO;

public class WorkSessionDto
{
    public int? UserId { get; set; }
    [Required]
    public DateTime Start { get; set; }
    public DateTime? End { get; set; }
    public int? LocationId { get; set; }
    public int? ProjectId { get; set; }
}