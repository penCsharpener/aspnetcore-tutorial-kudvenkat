using kudvenkat.DataAccess.Models.Seeding;
using kudvenkat.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace kudvenkat.DataAccess.Models {
    public class AppDbContext : DbContext {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {

        }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.SeedEmployees();
        }
    }
}
