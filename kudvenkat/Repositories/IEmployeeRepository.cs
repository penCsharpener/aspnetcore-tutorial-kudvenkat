﻿using kudvenkat.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kudvenkat.Repositories {
    public interface IEmployeeRepository {
        Employee GetEmployee(int Id);
        IEnumerable<Employee> GetEmployees();
        Employee Add(Employee employee);
    }
}
