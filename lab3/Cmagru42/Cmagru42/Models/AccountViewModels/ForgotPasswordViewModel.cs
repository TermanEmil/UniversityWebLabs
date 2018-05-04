using System;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [MaxLength(124)]
        public string Email { get; set; }
    }
}
