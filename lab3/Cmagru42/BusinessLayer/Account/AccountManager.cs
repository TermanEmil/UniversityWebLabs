using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLayer.Account.Models;
using BusinessLayer.Tools;
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

        public async Task<UserRegisterResponse> UserRegister(
            RegistrationData regData)
        {
            var task = _context.ApplicationUsers
                .FirstOrDefaultAsync(x =>
                                x.UserName.Equals(regData.UserName) &&
                                x.Email.Equals(regData.Email));

            if (task.Result != null)
                return new UserRegisterResponse()
                {
                    Status = ERegStatus.UserAlreadyExists,
                    AlreadyExistingUser = task.Result
                };

            var applicationUser = _mapper.Map<RegistrationData, ApplicationUser>(regData);
            applicationUser.SetPassword(new HashingTools(), regData.Password);

            _context.Add(applicationUser);
            await _context.SaveChangesAsync();

            return new UserRegisterResponse()
            {
                Status = ERegStatus.Success
            };
        }


    }
}
