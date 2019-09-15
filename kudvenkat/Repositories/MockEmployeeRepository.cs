using kudvenkat.DataAccess.Models;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kudvenkat.Repositories {
    public class MockEmployeeRepository : IEmployeeRepository {

        private List<Employee> _employeeList;

        public MockEmployeeRepository() {
            _employeeList = new List<Employee>() {
                new Employee() { Id = 1, Name = "Mary", Department = Dept.HR, Email = "mary@pragim.com" },
                new Employee() { Id = 2, Name = "John", Department = Dept.IT, Email = "john@pragim.com" },
                new Employee() { Id = 3, Name = "Sam", Department = Dept.IT, Email = "sam@pragim.com" },
            };
        }

        public Employee Add(Employee employee) {
            employee.Id = _employeeList.Max(x => x.Id) + 1;
            _employeeList.Add(employee);
            return employee;
        }

        public Employee Delete(int id) {
            var employee = _employeeList.Find(x => x.Id == id);
            if (employee != null) {
                _employeeList.Remove(employee);
            }
            return employee;
        }

        public Employee GetEmployee(int Id) {
            return _employeeList.Find(x => x.Id == Id);
        }

        public IEnumerable<Employee> GetAllEmployees() {
            return _employeeList;
        }

        public Employee Update(Employee employeeChanges) {
            var employee = _employeeList.Find(x => x.Id == employeeChanges.Id);
            if (employee != null) {
                employee.Name = employeeChanges.Name;
                employee.Email = employeeChanges.Email;
                employee.Department = employeeChanges.Department;
            }
            return employee;
        }
    }
}
