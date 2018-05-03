using System;
namespace DataLayer
{
    public class UserSettings
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public bool SendEmailNotifs { get; set; } = true;
    }
}
