using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using DietDoHongTran.Models; // Giả sử ApplicationUser nằm trong đây
using System.Threading.Tasks;
using System.Linq;
using DietDoHongTran.ViewModels;
using Microsoft.EntityFrameworkCore; // Giả sử ManageUserRolesViewModel nằm trong đây

namespace DietDoHongTran.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)] // Chỉ admin mới có quyền truy cập
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManagementController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Hiển thị danh sách người dùng
        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        // Hiển thị chi tiết người dùng
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.Roles = roles;
            return View(user);
        }

        // GET: Quản lý vai trò của người dùng
        public async Task<IActionResult> ManageRoles(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var allRoles = await _roleManager.Roles.ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(user);

            // Cập nhật NormalizedName cho các vai trò chưa có
            foreach (var role in allRoles)
            {
                if (string.IsNullOrEmpty(role.NormalizedName))
                {
                    role.NormalizedName = role.Name.ToUpper();
                    await _roleManager.UpdateAsync(role);
                }
            }

            var model = new ManageUserRolesViewModel
            {
                UserId = user.Id,
                Roles = allRoles
                    .Where(r => !string.IsNullOrEmpty(r.Name)) // Tránh lỗi nếu Role.Name null
                    .Select(r => new RoleSelection
                    {
                        RoleName = r.Name,
                        Selected = userRoles.Contains(r.Name)
                    }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles([Bind("UserId,Roles")] ManageUserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            // Lấy các vai trò hiện có của người dùng
            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError("", "Không thể xóa vai trò hiện tại.");
                return View(model);
            }

            // Thêm vai trò được chọn và cập nhật NormalizedName cho từng vai trò
            var selectedRoles = model.Roles.Where(r => r.Selected).Select(r => r.RoleName);

            foreach (var roleName in selectedRoles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null && string.IsNullOrEmpty(role.NormalizedName))
                {
                    role.NormalizedName = role.Name.ToUpper();
                    await _roleManager.UpdateAsync(role);
                }
            }

            var addResult = await _userManager.AddToRolesAsync(user, selectedRoles);
            if (!addResult.Succeeded)
            {
                ModelState.AddModelError("", "Không thể cập nhật vai trò.");
                return View(model);
            }

            return RedirectToAction(nameof(Details), new { id = model.UserId });
        }


        // Xóa người dùng
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Lỗi khi xóa người dùng");
                return View(user);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
