using NBApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NBApp.ViewModels;

namespace NBApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private UserManager<NBAppUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;


        public UserController(UserManager<NBAppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {

            List<NBAppUser> users = new List<NBAppUser>();
            foreach (NBAppUser user in _userManager.Users)
            {
                user.Roles = await _userManager.GetRolesAsync(user);
                users.Add(user);
            }
            UserViewModel model = new UserViewModel
            {
                Users = users,
                Roles = _roleManager.Roles.ToList()
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

    }
}
