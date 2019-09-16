﻿using kudvenkat.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kudvenkat.Controllers {
    public class AdministrationController : Controller {
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager) {
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult CreateRole() {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model) {
            if (ModelState.IsValid) {
                var identityRole = new IdentityRole() {
                    Name = model.RoleName,
                };
                var result = await _roleManager.CreateAsync(identityRole);

                if (result.Succeeded) {
                    return RedirectToAction("index", "home");
                }

                foreach (var error in result.Errors) {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

    }
}
