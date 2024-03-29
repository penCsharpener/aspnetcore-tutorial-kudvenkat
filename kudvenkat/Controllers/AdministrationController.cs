﻿using kudvenkat.DataAccess.Models;
using kudvenkat.Utils;
using kudvenkat.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace kudvenkat.Controllers {

    [Authorize(Policy = nameof(AuthPolicies.AdminRolePolicy))]
    public class AdministrationController : Controller {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdministrationController> _logger;

        public AdministrationController(RoleManager<IdentityRole> roleManager,
                                        UserManager<ApplicationUser> userManager,
                                        ILogger<AdministrationController> logger) {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model) {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null) {
                ViewBag.ErrorMessage = $"User with Id = {model.UserId} cannot be found";
                return View("NotFound");
            }

            var claims = await _userManager.GetClaimsAsync(user);
            var result = await _userManager.RemoveClaimsAsync(user, claims);

            if (!result.Succeeded) {
                ModelState.AddModelError("", "Cannot remove user existing claims");
                return View(model);
            }

            result = await _userManager.AddClaimsAsync(user,
                model.Claims.Where(x => x.IsSelected)
                            .Select(x => new Claim(x.ClaimType, x.IsSelected.ToString().ToLower())));

            if (!result.Succeeded) {
                ModelState.AddModelError("", "Cannot add selected claims to user");
                return View(model);
            }

            return RedirectToAction(nameof(EditUser), new { Id = model.UserId });
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string userId) {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var existingUserClaims = await _userManager.GetClaimsAsync(user);

            var model = new UserClaimsViewModel() {
                UserId = userId,
            };

            foreach (var claim in ClaimsStore.AllClaims) {
                var userClaim = new UserClaim() {
                    ClaimType = claim.Type,
                };

                // If the user has the claim, set IsSelected property to true, so the checkbox
                // next to the claim is checked on the UI
                if (existingUserClaims.Any(x => x.Type == claim.Type && x.Value == "true")) {
                    userClaim.IsSelected = true;
                }

                model.Claims.Add(userClaim);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = nameof(AuthPolicies.EditRolePolicy))]
        public async Task<IActionResult> ManageUserRoles(List<IdNameSelectedBase> model, string userId) {

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded) {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }

            result = await _userManager.AddToRolesAsync(user, model.Where(x => x.IsSelected).Select(x => x.Name));

            if (!result.Succeeded) {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }

            return RedirectToAction(nameof(EditUser), new { Id = userId });
        }

        [HttpGet]
        [Authorize(Policy = nameof(AuthPolicies.EditRolePolicy))]
        public async Task<IActionResult> ManageUserRoles(string userId) {
            ViewBag.userId = userId;

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var model = new List<IdNameSelectedBase>();

            foreach (var role in _roleManager.Roles) {
                var vm = new IdNameSelectedBase() {
                    Id = role.Id,
                    Name = role.Name,
                };

                vm.IsSelected = await _userManager.IsInRoleAsync(user, role.Name);

                model.Add(vm);
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = nameof(AuthPolicies.DeleteRolePolicy))]
        public async Task<IActionResult> DeleteRole(string id) {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null) {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            } else {
                try {

                    var result = await _roleManager.DeleteAsync(role);

                    if (result.Succeeded) {
                        return RedirectToAction(nameof(AdministrationController.ListRoles));
                    }

                    foreach (var error in result.Errors) {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View(nameof(AdministrationController.ListRoles));
                } catch (DbUpdateException ex) {
                    _logger.LogError($"Error deleting role {ex}");

                    ViewBag.ErrorTitle = $"{role.Name} role is in use";
                    ViewBag.ErrorMessage = $"{role.Name} role cannot be deleted as there are users in this " +
                        $"role. If you want to delete this role, please remove the users from " +
                        $"the role and then try to delete.";
                    return View("Error");
                }
            }
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
                Claims = userClaims.Select(c => $"{c.Type}: {c.Value}").ToList(),
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
                    return RedirectToAction(nameof(ListUsers));
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
                    return RedirectToAction(nameof(ListRoles), ControllerNameOutput.ToString(nameof(AdministrationController)));
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

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId) {
            ViewBag.roleId = roleId;

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null) {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            var model = new List<IdNameSelectedBase>();
            foreach (var user in _userManager.Users) {
                var userRoleViewModel = new IdNameSelectedBase() {
                    Id = user.Id,
                    Name = user.UserName
                };

                userRoleViewModel.IsSelected = await _userManager.IsInRoleAsync(user, role.Name);

                model.Add(userRoleViewModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<IdNameSelectedBase> model, string roleId) {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null) {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++) {
                var user = await _userManager.FindByIdAsync(model[i].Id);
                IdentityResult result = null;

                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name))) {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                } else if (!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name)) {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                } else {
                    continue;
                }

                if (result.Succeeded) {
                    if (i < (model.Count - 1)) {
                        continue;
                    } else {
                        return RedirectToAction(nameof(EditRole), new { Id = roleId });
                    }
                }
            }

            return RedirectToAction(nameof(EditRole), new { Id = roleId });
        }
    }
}
