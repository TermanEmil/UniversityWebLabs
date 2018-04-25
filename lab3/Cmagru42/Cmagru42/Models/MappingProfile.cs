using DataLayer.AppUser;
using Presentation.Models.AccountViewModels;

namespace Presentation.Models
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterViewModel, ApplicationUser>();
        }
    }
}
