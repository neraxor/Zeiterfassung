using System.ComponentModel.DataAnnotations;

namespace Zeiterfassung.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Username { get; set; }
    [Required]
    public string PasswordHash { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public int WorkingHoursWeekly { get; set; }
    public List<Location> Locations { get; set; }
    public List<Project> Projects { get; set; }
}