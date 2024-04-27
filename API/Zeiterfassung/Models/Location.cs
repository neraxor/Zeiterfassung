using System.ComponentModel.DataAnnotations;

namespace Zeiterfassung.Models;

public class Location
{
    [Key]
    public string Id { get; set; }
    [Required]
    public string Description { get; set; }
}