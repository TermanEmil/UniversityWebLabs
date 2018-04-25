using System;
using DataLayer.AppUser;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.DB
{
    public class CmagruDBContext : IdentityDbContext<ApplicationUser>
    {
        public CmagruDBContext(DbContextOptions<CmagruDBContext> options)
            : base(options)
        {
        }

        public DbSet<ImgUpload> ImgUploads { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}
