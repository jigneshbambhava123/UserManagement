using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Services;
using UserManagement.Services.Interfaces;
using UserManagement.ViewModels;

namespace UserManagement.Controllers;

[Authorize]
public class UserController : Controller
{
    private readonly IUserApiService _userApiService;
    private readonly IEmailService _emailService;
    public UserController(IUserApiService userApiService,IEmailService emailService)
    {
        _userApiService = userApiService;
        _emailService = emailService;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        return View();
    }

    public async Task<IActionResult> UserList(int page = 1, int pageSize = 5, string search = "", string sortColumn = "Id", string sortDirection = "asc")
    {
        var allUsers = await _userApiService.GetAllUsersAsync();

          // Search logic
        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            allUsers = allUsers.Where(u =>
                (u.Firstname?.ToLower().Contains(search) ?? false) ||
                (u.Lastname?.ToLower().Contains(search) ?? false) ||
                (u.Email?.ToLower().Contains(search) ?? false) ||
                (u.RoleName?.ToLower().Contains(search) ?? false)
            );
        }

        // Sorting logic
        allUsers = sortColumn switch
        {
            "Firstname" => sortDirection == "asc" ? allUsers.OrderBy(u => u.Firstname) : allUsers.OrderByDescending(u => u.Firstname),
            "Lastname" => sortDirection == "asc" ? allUsers.OrderBy(u => u.Lastname) : allUsers.OrderByDescending(u => u.Lastname),
            "RoleName" => sortDirection == "asc" ? allUsers.OrderBy(u => u.RoleName) : allUsers.OrderByDescending(u => u.RoleName),
            _ => sortDirection == "asc" ? allUsers.OrderBy(u => u.Id) : allUsers.OrderByDescending(u => u.Id),
        };

        var totalUsers = allUsers.Count();

        var users = allUsers.Skip((page - 1) * pageSize)
                            .Take(pageSize) 
                            .ToList();

        return PartialView("_UserList", new PaginatedList<UserViewModel>(users, totalUsers, page, pageSize));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _userApiService.GetUserByIdAsync(id);
        return View(user);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Edit(UserViewModel user)
    {
        if (ModelState.IsValid)
        {
            var (success, message) = await _userApiService.UpdateUserAsync(user);

            if (success)
            {
                TempData["success"] = message; 
                return RedirectToAction("Index");
            }

            TempData["error"] = message; 
            return View(user);
        }

        return View(user);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View();

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(UserViewModel user)
    {
        if (ModelState.IsValid)
        {
            var (success, message) = await _userApiService.CreateUserAsync(user);

            if (success)
            {
                // Send account details email
                await _emailService.SendAccountDetailsEmail(user.Email, user.Firstname, user.Password);
                TempData["success"] = message; 
                return RedirectToAction("Index");
            }

            TempData["error"] = message; 
            return View(user);
        }
        return View(user);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message) = await _userApiService.DeleteUserAsync(id);

        return Json(new { success , message });
        
    }
}
