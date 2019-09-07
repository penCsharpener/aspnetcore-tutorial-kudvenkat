using kudvenkat.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace kudvenkat.DataAccess.Models.Seeding {
    public static class SeedEmployeeData {
        public static void SeedEmployees(this ModelBuilder modelBuilder) {
            modelBuilder.Entity<Employee>().HasData(
                new Employee() { Id = 1, Name = "Mary", Department = Dept.HR, Email = "mary@pragim.com" },
                new Employee() { Id = 2, Name = "John", Department = Dept.IT, Email = "john@pragim.com" },
                new Employee() { Id = 3, Name = "Sam", Department = Dept.IT, Email = "sam@pragim.com" }
                );
        }
    }
}
