using System.ComponentModel.DataAnnotations;

namespace Zeiterfassung.Data.DTO;

public class UserRegisterDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; }
    [Required]
    public string Username { get; set; }
}
