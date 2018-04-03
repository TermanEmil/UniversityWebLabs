using System;
using Microsoft.EntityFrameworkCore;

namespace Web.Models
{
    public class CmagruContext : DbContext
    {
        public CmagruContext(DbContextOptions<CmagruContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}
