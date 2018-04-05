using AutoMapper;
using DataLayer.DB;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
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
