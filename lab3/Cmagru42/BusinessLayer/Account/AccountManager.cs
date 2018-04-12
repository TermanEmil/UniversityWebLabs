using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLayer.Account.Models;
using DataLayer.AppUser;
using DataLayer.DB;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Account
{
    public class AccountManager
    {
        private readonly CmagruDBContext _context;
        private readonly IMapper _mapper;

        public AccountManager(CmagruDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        //public async Task<UserLoginResponse> UserLogin(LoginData loginData)
        //{
        //    //var ue = loginData.UsrNameOrEmail;
        //    //var user = await _context.ApplicationUsers
        //    //                         .FirstOrDefaultAsync(x =>
        //    //                                              x.UserName.Equals(ue) ||
        //    //                                              x.Email.Equals(ue));
        //    //if (user == null)
        //    //    return new UserLoginResponse() { Status = ELoginStatus.UserNotFound };

        //    //var hasher = new HashingTools();
        //    //var verifyRs = hasher.VerifyHashedPassword(user.Password, loginData.Password);

        //    //if (verifyRs != PasswordVerificationResult.Success)
        //        //return new UserLoginResponse() { Status = ELoginStatus.PasswordNoMatch };

        //    return new UserLoginResponse()
        //    {
        //        Status = ELoginStatus.Success
        //        //FoundUser = user
        //    };
        //}

        public async Task<UserRegisterResponse> UserRegister(RegistrationData regData)
        {
            //var user = await _context.ApplicationUsers
            //    .FirstOrDefaultAsync(x =>
            //                         x.UserName.Equals(regData.UserName) ||
            //                         x.Email.Equals(regData.Email));

            //if (user != null)
            //    return new UserRegisterResponse()
            //    {
            //        Status = ERegStatus.UserAlreadyExists,
            //        AlreadyExistingUser = user
            //    };

            //var applicationUser = _mapper.Map<RegistrationData, ApplicationUser>(regData);
            //applicationUser.SetPassword(new HashingTools(), regData.Password);
            //applicationUser.LastChange = DateTime.UtcNow;

            //_context.Add(applicationUser);
            //await _context.SaveChangesAsync();

            return new UserRegisterResponse()
            {
                Status = ERegStatus.Success
            };
        }


    }
}
