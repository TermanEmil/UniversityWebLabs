using System;
using BusinessLayer.Account.Models;
using DataLayer.AppUser;
using Presentation.Models.AccountViewModels;

namespace Presentation.Models
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterViewModel, RegistrationData>();
            CreateMap<RegistrationData, ApplicationUser>();
            CreateMap<RegisterViewModel, ApplicationUser>();
            CreateMap<LoginViewModel, LoginData>();
        }
    }
}
