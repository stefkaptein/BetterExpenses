using System.ComponentModel.DataAnnotations;

namespace BetterExpenses.Common.DTO.Auth;

public class RegisterModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [Compare(nameof(ConfirmPassword), ErrorMessage = "Passwords are not equal")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = null!;
    
    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords are not equal")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = null!;
}