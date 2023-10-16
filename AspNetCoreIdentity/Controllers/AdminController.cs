using AspNetCoreIdentity.Entities;
using AspNetCoreIdentity.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentity.Controllers
{
    [Authorize(Roles ="Admin,SuperAdmin")]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> List()
        {
            var filterusers = new List<AppUser>();

            var users = await _userManager.Users.ToListAsync();
            foreach (var user in users)
            {
                var userroles = await _userManager.GetRolesAsync(user);
                if (!userroles.Contains("SuperAdmin"))
                    filterusers.Add(user);
            }

            return View(filterusers);
        }
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> AssignRole(string id)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(x => x.Id == id);
            var userRoles = await _userManager.GetRolesAsync(user);
            var roles = await _roleManager.Roles.ToListAsync();

            var model = new RoleAssignSendModel();
            var list = new List<RoleAssingListModel>();

            foreach (var role in roles)
            {
                list.Add(new()
                {
                    RoleId = role.Id,
                    Name = role.Name,
                    Exist = userRoles.Contains(role.Name)
                });
            }

            model.Roles = list;
            model.UserId = id;

            return View(model);
        }
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> AssignRole(RoleAssignSendModel model)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(x => x.Id == model.UserId);
            var userroles = await _userManager.GetRolesAsync(user);

            foreach (var role in model.Roles)
            {
                if (role.Exist)
                {
                    if (!userroles.Contains(role.Name))
                        await _userManager.AddToRoleAsync(user, role.Name);
                }
                else
                {
                    if (userroles.Contains(role.Name))
                        await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
            }

            return RedirectToAction("List");
        }
    }
}
