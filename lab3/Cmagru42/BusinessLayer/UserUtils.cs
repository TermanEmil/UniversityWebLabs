using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataLayer.AppUser;
using DataLayer.DB;
using Microsoft.AspNetCore.Identity;

namespace BusinessLayer
{
    public class UserUtils
    {
        /// <summary>
        /// Get the first user id with the given role.
        /// </summary>
        public static string GetUserRoleId(CmagruDBContext dBContext, string roleName)
        {
            var role = dBContext.Roles.First(x => x.NormalizedName == roleName.ToUpper());
            var userRole = dBContext.UserRoles.FirstOrDefault(x => x.RoleId == role.Id);
            return userRole.UserId;
        }

        public static string GetUserRole(CmagruDBContext dBContext, ApplicationUser user)
        {
            var role = dBContext.UserRoles.FirstOrDefault(x => x.UserId == user.Id);
            if (role == null)
                return null;
            return dBContext.Roles.First(x => x.Id == role.RoleId).Name;
        }
    }
}
