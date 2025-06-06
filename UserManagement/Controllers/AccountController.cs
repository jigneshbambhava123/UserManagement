using Microsoft.AspNetCore.Mvc;

namespace UserManagement.Controllers;

public class AccountController: Controller
{
    public AccountController()
    {
    }

    // GET : Login Page
    public IActionResult Login()
    {
        return View();
    }
}
