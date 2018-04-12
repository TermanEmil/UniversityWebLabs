using System;
using DataLayer.AppUser;

namespace BusinessLayer.Account.Models
{
    public enum ELoginStatus
    {
        Success,
        UserNotFound,
        PasswordNoMatch
    }

    public class UserLoginResponse : IAppResponse
    {
        public bool Success { get { return Status == ELoginStatus.Success; } }

        public ELoginStatus Status { get; set; } = ELoginStatus.Success;
        public ApplicationUser FoundUser { get; set; } = null;
    }
}
