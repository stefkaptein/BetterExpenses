using System.ComponentModel.DataAnnotations;

namespace BetterExpenses.API.Models.Accounts;

public class RegisterModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public required string Email { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public required string Password { get; set; }
}