using System;
using System.Linq;
using DataLayer.AppUser;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DataLayer;

namespace DataLayer.DB
{
    public class SeedDB
    {
        public static void Init(IServiceProvider serviceProvider)
        {
            var options = serviceProvider.GetRequiredService<DbContextOptions<CmagruDBContext>>();
            var context = new CmagruDBContext(options);

            if (context.Users.Any())
                return;

            context.Users.AddRange(
                new ApplicationUser()
                {
                    UserName = "Emil",
                    Email = "terman.emil@gmail.com"
                },
                new ApplicationUser()
                {
                    UserName = "ViorelBostan",
                    Email = "viorel.bostan@alternosfera.md"
                },
                new ApplicationUser()
                {
                    UserName = "Hooin Kyoma",
                    Email = "おかべ.りんたろう@future.com"
                }
            );
            context.SaveChanges();
        }
    }
}
