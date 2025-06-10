using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Services;
using UserManagement.ViewModels;

namespace UserManagement.Controllers;

[Authorize]
public class UserController : Controller
{
    private readonly IUserApiService _userApiService;
    public UserController(IUserApiService userApiService)
    {
        _userApiService = userApiService;
    }


    public async Task<IActionResult> Index()
    {
        var users = await _userApiService.GetAllUsersAsync();
        return View(users);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _userApiService.GetUserByIdAsync(id);
        return View(user);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Edit(User user)
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
    public async Task<IActionResult> Create(User user)
    {
        if (ModelState.IsValid)
        {
            var (success, message) = await _userApiService.CreateUserAsync(user);

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
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _userApiService.DeleteUserAsync(id);

        TempData["success"] = "User Deleted Successfully!"; 
        return RedirectToAction("Index");
    }
}
