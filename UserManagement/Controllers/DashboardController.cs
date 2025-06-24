using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Models;
using UserManagement.Services.Interfaces;

namespace UserManagement.Controllers;

public class DashboardController : Controller
{
    private readonly IBookingService _bookingService;

    public DashboardController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

   [Authorize]
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> GetActiveUserCountJson()
    {
        var count = await _bookingService.GetTotalActiveUserCount();
        return Json(count);
    }

    public async Task<IActionResult> GetResourceUsed()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userid))
        {
            ModelState.AddModelError("id", "Invalid User ID.");
            return BadRequest(ModelState);
        }

        var history = await _bookingService.GetBookingHistoryAsync(userid); 

        return Json(history);
    }

}
