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

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id) {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null) {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            } else {
                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded) {
                    return RedirectToAction(nameof(AdministrationController.ListUsers));
                }

                foreach (var error in result.Errors) {
                    ModelState.AddModelError("", error.Description);
                }

                return View(nameof(AdministrationController.ListUsers));
            }

        }

        [HttpGet]
        public IActionResult ListUsers() {
            var users = _userManager.Users;
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id) {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null) {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");

            }

            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new EditUserViewModel() {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                City = user.City,
                Claims = userClaims.Select(c => c.Value).ToList(),
                Roles = userRoles.ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model) {
            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null) {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("NotFound");

            } else {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.City = model.City;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded) {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors) {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
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
