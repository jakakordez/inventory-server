using InventoryServer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationController : ControllerBase
{
    private readonly DatabaseContext db;

    public LocationController(DatabaseContext db)
    {
        this.db = db;
    }

    [HttpGet()]
    public async Task<List<LocationCategory>> GetLocations()
    {
        return await LoadRecursive(null);
    }

    private async Task<List<LocationCategory>> LoadRecursive(int? id)
    {
        var list = db.LocationCategories
            .Where(c => c.ParentId == id)
            .Include(c => c.Locations)
            .ToList();

        foreach (var subcategory in list)
        {
            subcategory.Subcategories = await LoadRecursive(subcategory.Id);
        }

        return list;
    }
}
