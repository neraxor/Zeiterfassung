using System.ComponentModel.DataAnnotations;

namespace Zeiterfassung.Models;

public class Project
{
    [Key]
    public string Id { get; set; }
    [Required]
    public string Name { get; set; }
    public string? Description { get; set; }
}