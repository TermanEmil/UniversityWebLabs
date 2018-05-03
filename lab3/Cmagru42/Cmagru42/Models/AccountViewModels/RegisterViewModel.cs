using System;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [MaxLength(32)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [MaxLength(124)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(
            124,
            MinimumLength = 3,
            ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation code don't match.")]
        public string ConfirmPassword { get; set; }
    }
}
