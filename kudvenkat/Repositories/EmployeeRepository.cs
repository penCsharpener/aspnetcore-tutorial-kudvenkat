using kudvenkat.DataAccess.Models;
using kudvenkat.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kudvenkat.Repositories {
    public class EmployeeRepository : IEmployeeRepository {

        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context) {
            _context = context;
        }

        public Employee Add(Employee employee) {
            _context.Employees.Add(employee);
            _context.SaveChanges();
            return employee;
        }

        public Employee Delete(int id) {
            var employee = _context.Employees.Find(id);
            if (employee != null) {
                _context.Employees.Remove(employee);
                _context.SaveChanges();
            }
            return employee;
        }

        public Employee GetEmployee(int Id) {
            return _context.Employees.Find(Id);
        }

        public IEnumerable<Employee> GetAllEmployees() {
            return _context.Employees;
        }

        public Employee Update(Employee employeeChanges) {
            var employee = _context.Employees.Attach(employeeChanges);
            employee.State = EntityState.Modified;
            _context.SaveChanges();
            return employeeChanges;
        }
    }
}
