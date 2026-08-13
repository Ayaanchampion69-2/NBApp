using NBApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NBApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using NBApp.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace NBApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private UserManager<NBAppUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly NBAppContext _context;


        public UserController(UserManager<NBAppUser> userManager, RoleManager<IdentityRole> roleManager, NBAppContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {

            List<NBAppUser> users = new List<NBAppUser>();
            foreach (NBAppUser user in _userManager.Users)
            {
                user.Roles = await _userManager.GetRolesAsync(user);
                users.Add(user);
            }

            var existingPermissions = await _context.FeaturePermissions.ToListAsync();

            UserViewModel model = new UserViewModel
            {
                Users = users,
                Roles = _roleManager.Roles.ToList(),
                Features = FeatureCatalog.FeatureKeys.Select(fk => new FeaturePermissionRow
                {
                    FeatureKey = fk,
                    AllowedRoles = existingPermissions
                        .Where(p => p.FeatureKey == fk)
                        .Select(p => p.RoleName)
                        .ToList()
                }).ToList()
            };


            return View(model);
        }

        [HttpPost]// delete user method
        public async Task<IActionResult> Delete(string id)
        {
            NBAppUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {//if failed basically
                    string errorMessage = "";
                    foreach (IdentityError error in result.Errors)
                    {
                        errorMessage += error.Description + " | ";
                    }
                    TempData["ErrorMessage"] = errorMessage;

                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddToRole(string userId, string roleName)
        {
            IdentityRole role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                TempData["ErrorMessage"] = $"Role '{role}' does not exist.";
            }
            else
            {
                NBAppUser user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    await _userManager.AddToRoleAsync(user, roleName);
                }


            }
            return RedirectToAction("Index");


        }
        [HttpPost]
        public async Task<IActionResult> RemoveFromRole(string userId, string roleName)
        {
            NBAppUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.RemoveFromRoleAsync(user, roleName);
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> CreateRole(string rolename)
        {
            await _roleManager.CreateAsync(new IdentityRole(rolename));

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdminRole()
        {
            await _roleManager.CreateAsync(new IdentityRole("Admin"));
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            await _roleManager.DeleteAsync(role);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePermissions(string featureKey, List<string> selectedRoles)
        {
            if (!FeatureCatalog.FeatureKeys.Contains(featureKey))
            {
                TempData["ErrorMessage"] = $"Unknown feature '{featureKey}'.";
                return RedirectToAction("Index");
            }

            selectedRoles ??= new List<string>();

            var existing = _context.FeaturePermissions.Where(p => p.FeatureKey == featureKey);
            _context.FeaturePermissions.RemoveRange(existing);

            foreach (var role in selectedRoles)
            {
                _context.FeaturePermissions.Add(new FeaturePermission
                {
                    FeatureKey = featureKey,
                    RoleName = role
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}