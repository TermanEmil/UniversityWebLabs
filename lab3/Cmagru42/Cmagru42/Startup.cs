using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataLayer.AppUser;
using DataLayer.DB;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Presentation
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddAutoMapper();

            services.AddWebOptimizer(pipeline =>
            {
                pipeline.MinifyCssFiles("~/css/site.css");
            });

            services.AddDbContext<CmagruDBContext>(options =>
                options.UseSqlite("Data Source=Cmagru.db",
                                  b => b.MigrationsAssembly("Presentation")));
            
            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<CmagruDBContext>()
                    .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 3;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
            });
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWebOptimizer();
            app.UseStaticFiles();
            app.UseMvc();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Page not found!");
            });
        }
    }
}
