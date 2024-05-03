using System.ComponentModel.DataAnnotations;

namespace Zeiterfassung.Models;

public class WorkSession
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int UserId { get; set; }
    public virtual User User { get; set; }
    [Required]
    public DateTime Start { get; set; }
    public DateTime? End { get; set; }
    public int? LocationId { get; set; }
    public virtual Location Location { get; set; }
    public int? ProjectId { get; set; }
    public virtual Project Project { get; set; }
}