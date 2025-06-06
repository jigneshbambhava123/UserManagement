using Microsoft.AspNetCore.Mvc;
using UserManagement.Services;
using UserManagement.ViewModels;

namespace UserManagement.Controllers;

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

    public async Task<IActionResult> Edit(int id)
    {
        var user = await _userApiService.GetUserByIdAsync(id);
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(User user)
    {
        if (ModelState.IsValid)
        {
            await _userApiService.UpdateUserAsync(user);
            return RedirectToAction("Index");
        }
        return View(user);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(User user)
    {
        if (ModelState.IsValid)
        {
            await _userApiService.CreateUserAsync(user);
            return RedirectToAction("Index");
        }
        return View(user);
    }

    public async Task<IActionResult> Delete(int id)
    {
        await _userApiService.DeleteUserAsync(id);
        return RedirectToAction("Index");
    }
}
