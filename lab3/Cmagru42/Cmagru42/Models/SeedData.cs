using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Web.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new CmagruContext(
                serviceProvider.GetRequiredService<DbContextOptions<CmagruContext>>()))
            {
                if (context.ApplicationUsers.Any())
                    return;

                context.ApplicationUsers.AddRange(
                    new ApplicationUser()
                    {
                        UserName = "Unicornslayer",
                        Email = "terman.emil@gmail.com"
                    },
                    new ApplicationUser()
                    {
                        UserName = "Vicu Bostanel",
                        Email = "vicu.bostanel@alternosfera.md"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
