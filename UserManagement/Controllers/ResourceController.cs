using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Services.Interfaces;
using UserManagement.ViewModels;

namespace UserManagement.Controllers;

[Authorize]
public class ResourceController : Controller
{
    private readonly IResourceService _resourceService;

    public ResourceController(IResourceService resourceService)
    {
        _resourceService = resourceService;
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        return View();
    }

    public async Task<IActionResult> ResourceList(int page = 1, int pageSize = 5, string search = "", string sortColumn = "Id", string sortDirection = "asc")
    {
        var allResources = await _resourceService.GetAllResourcesAsync();
          // Search logic
        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            allResources = allResources.Where(u =>
                (u.Name?.ToLower().Contains(search) ?? false) ||
                u.Quantity.ToString().Contains(search)
            );
        }

        // Sorting logic
        allResources = sortColumn switch
        {
            "Name" => sortDirection == "asc" ? allResources.OrderBy(u => u.Name) : allResources.OrderByDescending(u => u.Name),
            "Quantity" => sortDirection == "asc" ? allResources.OrderBy(u => u.Quantity) : allResources.OrderByDescending(u => u.Quantity),
            _ => sortDirection == "asc" ? allResources.OrderBy(u => u.Id) : allResources.OrderByDescending(u => u.Id),
        };

        var totalResources = allResources.Count();

        var resources = allResources.Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();

        return PartialView("_ResourceList", new PaginatedList<ResourceViewModel>(resources, totalResources, page, pageSize));
    }
    
    public IActionResult AddResource()
    {
        return PartialView("_AddResource", new ResourceViewModel());
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ResourceViewModel resourceViewModel)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage);
            return BadRequest("Validation Errors: " + string.Join(", ", errors));
        }

        var (success, message) = await _resourceService.CreateResourceAsync(resourceViewModel);

        return Json(new { success , message });
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditResource(int id)
    {
        var resource = await _resourceService.GetResourceByIdAsync(id);
        return PartialView("_EditResource", resource);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditResource(ResourceViewModel resourceViewModel)
    {
          if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage);
            return BadRequest("Validation Errors: " + string.Join(", ", errors));
        }

        var (success, message) = await _resourceService.UpdateResourceAsync(resourceViewModel);

        return Json(new { success , message });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message) = await _resourceService.DeleteResourceAsync(id);
        return Json(new { success , message });
    }
}
