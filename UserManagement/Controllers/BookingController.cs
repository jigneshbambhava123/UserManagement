using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Services.Interfaces;
using UserManagement.ViewModels;

namespace UserManagement.Controllers;

[Authorize]
public class BookingController : Controller
{
    private readonly IBookingService _bookingService;
    private readonly IResourceService _resourceService;

    public BookingController(IBookingService bookingService,IResourceService resourceService)
    {
        _bookingService = bookingService;
        _resourceService = resourceService;
    }

    [Authorize(Roles ="Admin,User")]
    public async Task<IActionResult> Index()
    {
        return View();
    }

    public async Task<IActionResult> BookResource()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userid))
        {
            ModelState.AddModelError("id", "Invalid User ID.");
            return BadRequest(ModelState);
        }
        return PartialView("_BookResource",new BookingViewModel{ Resources = (List<ResourceViewModel>)await _resourceService.GetAllResourcesAsync(), UserId = userid});
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BookResource(BookingViewModel bookingViewModel)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage);
            return BadRequest("Validation Errors: " + string.Join(", ", errors));
        }

        var (success, message) = await _bookingService.BookResourceAsync(bookingViewModel);

        return Json(new { success , message });
    }

    [Authorize(Roles ="Admin,User")]
    public async Task<IActionResult> ActiveBookings(int page = 1, int pageSize = 5)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userid))
        {
            ModelState.AddModelError("id", "Invalid User ID.");
            return BadRequest(ModelState);
        }
        var allActiveBookings = await _bookingService.GetActiveBookingsAsync(userid);
        var totalActiveBookings = allActiveBookings.Count();

        var bookings = allActiveBookings.Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();

        return PartialView("_ActiveBookingList", new PaginatedList<BookingViewModel>(bookings, totalActiveBookings, page, pageSize));
    }

    public async Task<IActionResult> BookingHistory(int page = 1, int pageSize = 5)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userid))
        {
            ModelState.AddModelError("id", "Invalid User ID.");
            return BadRequest(ModelState);
        }
        var allBookings = await _bookingService.GetBookingHistoryAsync(userid);
        var totalBookings = allBookings.Count();

        var bookings = allBookings.Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();

        return PartialView("_BookingHistoryList", new PaginatedList<BookingViewModel>(bookings, totalBookings, page, pageSize));
    }
}
