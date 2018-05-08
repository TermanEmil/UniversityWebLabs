using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLayer;
using BusinessLayer.Emailing;
using DataLayer.AppUser;
using DataLayer.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Authorization;
using Presentation.Services;

namespace Presentation
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper();
            services.AddWebOptimizer();

            services.AddDbContext<CmagruDBContext>(options =>
                options.UseSqlite("Data Source=Cmagru.db",
                                  b => b.MigrationsAssembly("Presentation")));
            
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
            })
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

            services.AddSingleton<IEmailConfiguration>(
                Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>());
            services.AddTransient<IEmailService, EmailService>();

            services.AddMvc();

            var serviceProvider = services.BuildServiceProvider();
            var roleInitializer = new InitRoles(
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>(),
                serviceProvider.GetRequiredService<UserManager<ApplicationUser>>(),
                Configuration);
            roleInitializer.InitAppRolesAsync().Wait();

			services.AddScoped<
		        IAuthorizationHandler,
		        ImgIsOwnerAuthorizationHandler>();

			services.AddScoped<
				IAuthorizationHandler,
			    ImgUploadAdministratorsAuthorizationHandler>();

			services.AddScoped<
				IAuthorizationHandler,
			    ImgOverlayUploadAuthorizationHandler>();

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWebOptimizer();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc();
            //app.UseMvcWithDefaultRoute();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Page not found!");
            });
        }
    }
}
