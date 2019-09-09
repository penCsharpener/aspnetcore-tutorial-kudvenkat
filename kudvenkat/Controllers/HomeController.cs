using kudvenkat.Models;
using kudvenkat.Repositories;
using kudvenkat.ViewModels;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace kudvenkat.Controllers {
    public class HomeController : Controller {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly HostingEnvironment _hostingEnvironment;

        public HomeController(IEmployeeRepository employeeRepository,
                              HostingEnvironment hostingEnvironment) {
            _employeeRepository = employeeRepository;
            _hostingEnvironment = hostingEnvironment;
        }

        [Route("")]
        [Route("home")]
        [Route("home/index")]
        public ViewResult Index() {
            return View(_employeeRepository.GetAllEmployees());
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
        public IActionResult Create(EmployeeCreateViewModel model) {
            if (ModelState.IsValid) {
                string uniqueFileName = null;
                if (model.Photo != null) {
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                    string filePath = Path.Combine(Path.Combine(_hostingEnvironment.WebRootPath, "img"), uniqueFileName);
                    model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
                }
                var newEmployee = new Employee() {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniqueFileName
                };

                _employeeRepository.Add(newEmployee);
                return RedirectToAction("details", new { id = newEmployee.Id });
            }
            return View();
        }
    }
}
