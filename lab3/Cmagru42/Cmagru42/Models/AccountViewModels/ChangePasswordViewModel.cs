using System;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models.AccountViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [Display(Name = "Current password")]
        [DataType(DataType.Password)]
        [StringLength(
            124,
            MinimumLength = 3,
            ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        public string CurrentPassword { get; set; }

        [Required]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        [StringLength(
            124,
            MinimumLength = 3,
            ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation code don't match.")]
        public string ConfirmPassword { get; set; }
    }
}
