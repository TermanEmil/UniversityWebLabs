using System;
using DataLayer.AppUser;

namespace BusinessLayer.Account.Models
{
    public enum ERegStatus
    {
        Success,
        UserAlreadyExists,
        Fail
    }

    public class UserRegisterResponse : IAppResponse
    {
        public bool Success
        {
            get { return Status == ERegStatus.Success; }
        }

        public ERegStatus Status = ERegStatus.Fail;
        public ApplicationUser AlreadyExistingUser;
    }
}
