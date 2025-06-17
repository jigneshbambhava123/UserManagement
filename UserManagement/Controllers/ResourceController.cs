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

    public async Task<IActionResult> ResourceList(int page = 1, int pageSize = 5)
    {
        var allResources = await _resourceService.GetAllResourcesAsync();
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
    public async Task<IActionResult> Create(ResourceViewModel resource)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage);
            return BadRequest("Validation Errors: " + string.Join(", ", errors));
        }

        var (success, message) = await _resourceService.CreateResourceAsync(resource);

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
    public async Task<IActionResult> EditResource(ResourceViewModel resource)
    {
          if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage);
            return BadRequest("Validation Errors: " + string.Join(", ", errors));
        }

        var (success, message) = await _resourceService.UpdateResourceAsync(resource);

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
