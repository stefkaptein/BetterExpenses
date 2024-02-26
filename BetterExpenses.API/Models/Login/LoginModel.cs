using System.ComponentModel.DataAnnotations;

namespace BetterExpenses.API.Models.Login;

public class LoginModel
{
    [Required]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }

    public bool RememberMe { get; set; }
}