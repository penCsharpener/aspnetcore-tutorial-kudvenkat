using kudvenkat.Models;
using kudvenkat.Repositories;
using kudvenkat.ViewModels;
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

        [Route("")]
        [Route("home")]
        [Route("home/index")]
        public ViewResult Index() {
            return View(_employeeRepository.GetEmployees());
        }

        [Route("home/details/{id?}")]
        public ViewResult Details(int? id) {
            var vm = new HomeDetailsViewModel() {
                Employee = _employeeRepository.GetEmployee(id ?? 1),
                PageTitle = "Details Page of "
            };
            vm.PageTitle += vm.Employee?.Name;
            return View(vm);
        }

        [HttpGet]
        public ViewResult Create() {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Employee employee) {
            if (ModelState.IsValid) {
                var newEmployee = _employeeRepository.Add(employee);
                return RedirectToAction("details", new { id = newEmployee.Id });
            }
            return View();
        }
    }
}
