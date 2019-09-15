using kudvenkat.DataAccess.Models.Seeding;
using kudvenkat.DataAccess.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace kudvenkat.DataAccess.Models {
    public class AppDbContext : IdentityDbContext<ApplicationUser> {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {

        }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.SeedEmployees();
        }
    }
}
