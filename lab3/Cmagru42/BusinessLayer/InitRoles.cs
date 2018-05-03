using System;
using System.Threading.Tasks;
using DataLayer.AppUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace BusinessLayer
{
    public class InitRoles
    {
        public readonly string[] roleNames = { "Admin", "ImgOverlayer", "Member" };
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public InitRoles(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task InitAppRolesAsync()
        {
            foreach (var roleName in roleNames)
            {
                if (await _roleManager.RoleExistsAsync(roleName))
                    continue;
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            await CreateUserAsync("Admin", "Admin");
            await CreateUserAsync("ImgOverlayer", "ImgOverlayer");
        }

        private async Task CreateUserAsync(string configIndex, string roleName)
        {
            var newUser = new ApplicationUser()
            {
                UserName = _configuration[configIndex + ":UserName"],
                Email = _configuration[configIndex + ":Email"],
                EmailConfirmed = true
            };

            var user = await _userManager.FindByEmailAsync(newUser.Email);
            if (user == null)
            {
                var createdUser = await _userManager
                    .CreateAsync(newUser, _configuration[configIndex + ":Password"]);
                if (createdUser.Succeeded)
                    await _userManager.AddToRoleAsync(newUser, roleName);
            }
        }
    }
}
