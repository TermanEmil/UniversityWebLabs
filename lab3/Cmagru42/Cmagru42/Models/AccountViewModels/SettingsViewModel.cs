using System;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models.AccountViewModels
{
    public class SettingsViewModel
    {
        public bool SendNotifsOnEmail { get; set; }

        [StringLength(
            32,
            MinimumLength = 2,
            ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        public string NewUserName { get; set; }
        public string CurrentUserName { get; set; }

        [MaxLength(124)]
        [Display(Name = "Email")]
        public string NewEmail { get; set; }
        public string CurrentEmail { get; set; }
    }
}
