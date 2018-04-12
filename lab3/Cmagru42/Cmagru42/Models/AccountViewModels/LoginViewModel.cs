using System;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email or Username")]
        public string EmailOrUsername { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remeber me")]
        public bool RememberMe { get; set; }
    }
}
