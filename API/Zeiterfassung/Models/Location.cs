using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeiterfassung.Models;

public class Location
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Description { get; set; }
    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; } 
}