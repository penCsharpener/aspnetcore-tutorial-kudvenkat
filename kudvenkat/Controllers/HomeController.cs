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
            var employee = _employeeRepository.GetEmployee(id ?? 1);

            if (employee == null) {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", id.Value);
            }

            var vm = new HomeDetailsViewModel() {
                Employee = employee,
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
                var uniqueFileName = ProcessUploadedFile(model);
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

        [HttpGet]
        public ViewResult Edit(int id) {
            var employee = _employeeRepository.GetEmployee(id);
            var vm = new EmployeeEditViewModel() {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };
            return View(vm);
        }

        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model) {
            if (ModelState.IsValid) {
                var employee = _employeeRepository.GetEmployee(model.Id);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;

                if (model.Photo != null) {
                    if (model.ExistingPhotoPath != null) {
                        var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "img", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    employee.PhotoPath = ProcessUploadedFile(model);
                }

                _employeeRepository.Update(employee);
                return RedirectToAction("index");
            }
            return View();
        }

        private string ProcessUploadedFile(EmployeeCreateViewModel model) {
            string uniqueFileName = null;
            if (model.Photo != null) {
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(Path.Combine(_hostingEnvironment.WebRootPath, "img"), uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create)) {
                    model.Photo.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}
