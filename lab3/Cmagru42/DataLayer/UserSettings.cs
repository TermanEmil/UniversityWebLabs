using System;
namespace DataLayer
{
    public class UserSettings
    {
        public string UserId { get; set; }
        public bool SendEmailNotifs { get; set; } = true;
    }
}
