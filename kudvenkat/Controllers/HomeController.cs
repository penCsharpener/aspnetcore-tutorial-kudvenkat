using kudvenkat.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kudvenkat.Controllers {
    public class HomeController : Controller {
        private readonly IEmployeeRepository _employeeRepository;

        public HomeController(IEmployeeRepository employeeRepository) {
            _employeeRepository = employeeRepository;
        }

        public ViewResult Index() {
            return View(_employeeRepository.GetEmployees());
        }

        public ViewResult Details(int id) {
            return View(_employeeRepository.GetEmployee(id));
        }
    }
}
