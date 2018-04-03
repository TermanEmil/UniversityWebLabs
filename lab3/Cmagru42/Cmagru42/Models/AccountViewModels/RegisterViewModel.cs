using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Username")]
        [StringLength(
            30,
            MinimumLength = 3,
            ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(
            124,
            MinimumLength = 6,
            ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation code don't match.")]
        public string ConfirmPassword { get; set; }
    }
}
