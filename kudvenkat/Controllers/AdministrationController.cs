﻿using kudvenkat.DataAccess.Models;
using kudvenkat.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kudvenkat.Controllers {

    public class AdministrationController : Controller {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager,
                                        UserManager<ApplicationUser> userManager) {
            _roleManager = roleManager;
            _userManager = userManager;
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
                    return RedirectToAction(nameof(ListRoles), "Administration");
                }

                foreach (var error in result.Errors) {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ListRoles() {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id) {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null) {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }

            var model = new EditRoleViewModel() {
                Id = role.Id,
                RoleName = role.Name,
            };

            foreach (var user in _userManager.Users) {
                if (await _userManager.IsInRoleAsync(user, role.Name)) {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model) {

            var role = await _roleManager.FindByIdAsync(model.Id);

            if (role == null) {
                ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                return View("NotFound");
            } else {
                role.Name = model.RoleName;
                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded) {
                    return RedirectToAction(nameof(ListRoles));
                }

                foreach (var error in result.Errors) {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied() {
            return View();
        }
    }
}
