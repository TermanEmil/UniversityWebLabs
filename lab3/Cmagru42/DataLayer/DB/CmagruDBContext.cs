using System;
using DataLayer.AppUser;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.DB
{
    public class CmagruDBContext : DbContext
    {
        public CmagruDBContext(DbContextOptions<CmagruDBContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}
