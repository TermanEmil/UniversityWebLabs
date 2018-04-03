using System;
using Microsoft.AspNetCore.Identity;

namespace Web.Models
{
    public class ApplicationUser
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
