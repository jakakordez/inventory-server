using InventoryServer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly DatabaseContext db;

    public LocationsController(DatabaseContext db)
    {
        this.db = db;
    }

    [HttpGet()]
    public async Task<List<LocationCategory>> Get()
    {
        return await LoadRecursive(null);
    }

    private async Task<List<LocationCategory>> LoadRecursive(int? id)
    {
        var categories = await db.LocationCategories
            .Where(c => c.ParentId == id)
            .Include(c => c.Locations)
            .ToListAsync();

        foreach (var cat in categories)
        {
            cat.Subcategories = await LoadRecursive(cat.Id);
        }

        return categories;
    }

    [HttpGet("{id}/parts")]
    public async Task<List<Part>> GetParts(
        [FromRoute] int id)
    {
        return await db.Parts
            .Where(p => p.LocationId == id)
            .ToListAsync();
    }
}
