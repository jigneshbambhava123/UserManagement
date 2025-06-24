using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Helper;
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
    public async Task<IActionResult> ActiveBookings(int page = 1, int pageSize = 5, string search = "", string time="", string sortColumn = "Id", string sortDirection = "asc")
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userid))
        {
            ModelState.AddModelError("id", "Invalid User ID.");
            return BadRequest(ModelState);
        }
        var allActiveBookings = await _bookingService.GetActiveBookingsAsync(userid);

          // Search logic
        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            allActiveBookings = allActiveBookings.Where(u =>
                (u.ResourceName?.ToLower().Contains(search) ?? false) ||
                u.Quantity.ToString().Contains(search)
            );
        }

         // Time filter logic
        if (!string.IsNullOrEmpty(time) && time != "AllTime")
        {
            DateTime startDate;
            DateTime endDate;

            switch (time.ToLower())
            {
                case "today":
                    startDate = DateTime.Today;
                    endDate = DateTime.Today.AddDays(1).AddTicks(-1);
                    allActiveBookings = allActiveBookings.Where(b => b.FromDate >= startDate && b.FromDate <= endDate);
                    break;
                case "this_week":
                    var currentWeekStart = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
                    var currentWeekEnd = currentWeekStart.AddDays(7).AddTicks(-1);
                    allActiveBookings = allActiveBookings.Where(b => b.FromDate >= currentWeekStart && b.FromDate <= currentWeekEnd);
                    break;
                case "this_month":
                    var currentMonthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    var currentMonthEnd = currentMonthStart.AddMonths(1).AddTicks(-1);
                    allActiveBookings = allActiveBookings.Where(b => b.FromDate >= currentMonthStart && b.FromDate <= currentMonthEnd);
                    break;
            }
        }

        // Sorting logic
        allActiveBookings = sortColumn switch
        {
            "Name" => sortDirection == "asc" ? allActiveBookings.OrderBy(u => u.ResourceName) : allActiveBookings.OrderByDescending(u => u.ResourceName),
            "Quantity" => sortDirection == "asc" ? allActiveBookings.OrderBy(u => u.Quantity) : allActiveBookings.OrderByDescending(u => u.Quantity),
            "FromDate" => sortDirection == "asc" ? allActiveBookings.OrderBy(u => u.FromDate) : allActiveBookings.OrderByDescending(u => u.FromDate),
            "ToDate" => sortDirection == "asc" ? allActiveBookings.OrderBy(u => u.ToDate) : allActiveBookings.OrderByDescending(u => u.ToDate),
            _ => sortDirection == "asc" ? allActiveBookings.OrderBy(u => u.Id) : allActiveBookings.OrderByDescending(u => u.Id),
        };

        var totalActiveBookings = allActiveBookings.Count();


        var bookings = allActiveBookings.Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();

        return PartialView("_ActiveBookingList", new PaginatedList<BookingViewModel>(bookings, totalActiveBookings, page, pageSize));
    }

    public async Task<IActionResult> BookingHistory(int page = 1, int pageSize = 5, string search = "", string time="", string sortColumn = "Id", string sortDirection = "asc")
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userid))
        {
            ModelState.AddModelError("id", "Invalid User ID.");
            return BadRequest(ModelState);
        }
        var allBookings = await _bookingService.GetBookingHistoryAsync(userid);

          // Search logic
        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            allBookings = allBookings.Where(u =>
                (u.ResourceName?.ToLower().Contains(search) ?? false) ||
                u.Quantity.ToString().Contains(search)
            );
        }

        
         // Time filter logic
        if (!string.IsNullOrEmpty(time) && time != "AllTime")
        {
            DateTime startDate;
            DateTime endDate;

            switch (time.ToLower())
            {
                case "today":
                    startDate = DateTime.Today;
                    endDate = DateTime.Today.AddDays(1).AddTicks(-1);
                    allBookings = allBookings.Where(b => b.FromDate >= startDate && b.FromDate <= endDate);
                    break;
                case "this_week":
                    var currentWeekStart = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
                    var currentWeekEnd = currentWeekStart.AddDays(7).AddTicks(-1);
                    allBookings = allBookings.Where(b => b.FromDate >= currentWeekStart && b.FromDate <= currentWeekEnd);
                    break;
                case "this_month":
                    var currentMonthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    var currentMonthEnd = currentMonthStart.AddMonths(1).AddTicks(-1);
                    allBookings = allBookings.Where(b => b.FromDate >= currentMonthStart && b.FromDate <= currentMonthEnd);
                    break;
            }
        }

        // Sorting logic
        allBookings = sortColumn switch
        {
            "Name" => sortDirection == "asc" ? allBookings.OrderBy(u => u.ResourceName) : allBookings.OrderByDescending(u => u.ResourceName),
            "Quantity" => sortDirection == "asc" ? allBookings.OrderBy(u => u.Quantity) : allBookings.OrderByDescending(u => u.Quantity),
            "FromDate" => sortDirection == "asc" ? allBookings.OrderBy(u => u.FromDate) : allBookings.OrderByDescending(u => u.FromDate),
            "ToDate" => sortDirection == "asc" ? allBookings.OrderBy(u => u.ToDate) : allBookings.OrderByDescending(u => u.ToDate),
            _ => sortDirection == "asc" ? allBookings.OrderBy(u => u.Id) : allBookings.OrderByDescending(u => u.Id),
        };

        var totalBookings = allBookings.Count();

        var bookings = allBookings.Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();

        return PartialView("_BookingHistoryList", new PaginatedList<BookingViewModel>(bookings, totalBookings, page, pageSize));
    }
}
