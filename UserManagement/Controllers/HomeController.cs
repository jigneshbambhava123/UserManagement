using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Models;

namespace UserManagement.Controllers;

public class HomeController : Controller
{
    public HomeController()
    {
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [Route("Home/NotFound")]
    public IActionResult NotFound()
    {
        return View();
    }
}
