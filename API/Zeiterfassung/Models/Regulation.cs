using System.ComponentModel.DataAnnotations;

namespace Zeiterfassung.Models;

public class Regulation
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int WorkingHours { get; set; }
    [Required]
    public int BreakTime { get; set; }
}