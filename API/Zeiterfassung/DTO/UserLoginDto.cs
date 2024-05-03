using System.ComponentModel.DataAnnotations;

namespace Zeiterfassung.DTO;

public class UserLoginDto
{
    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; }
    [Required]
    public string Username { get; set; }
}