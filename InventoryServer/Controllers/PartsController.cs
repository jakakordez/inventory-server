using InventoryServer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PartsController : ControllerBase
{
    private readonly DatabaseContext db;

    public PartsController(DatabaseContext db)
    {
        this.db = db;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        [FromRoute] int id,
        [FromBody] Part part)
    {
        var existing = await db.Parts.FindAsync(id);
        if (existing == null)
        {
            return NotFound($"Part with id {id} not found");
        }
        existing.Name = part.Name;
        existing.Description = part.Description;
        existing.CategoryId = part.CategoryId;
        existing.LocationId = part.LocationId;

        db.Parts.Update(existing);
        await db.SaveChangesAsync();

        return Ok(existing);
    }

    [HttpGet("{id}/history")]
    public async Task<IActionResult> GetEntries(
        [FromRoute] int id)
    {
        return Ok(await db.StockEntries
            .Where(p => p.PartId == id)
            .ToListAsync());
    }

    [HttpPost("{id}/history")]
    public async Task<IActionResult> UpdateStock(
        [FromRoute] int id, 
        [FromBody] StockChange change)
    {
        if (change.NewLevel < 0)
        {
            return BadRequest("New level must be non-negative!");
        }
        var part = await db.Parts.FindAsync(id);
        if (part == null)
        {
            return NotFound($"Part with id {id} not found");
        }
        int levelChange = change.NewLevel - part.StockLevel;

        part.StockLevel = change.NewLevel;
        db.StockEntries.Add(new StockEntry()
        {
            PartId = id,
            Change = levelChange,
            Comment = change.Comment,
            Timestamp = DateTime.Now,
            UserId = 1
        });
        db.Parts.Update(part);
        await db.SaveChangesAsync();

        return Ok();
    }

    public class StockChange
    {
        required public int NewLevel { get; init; }
        public string? Comment { get; init; }
    }
}
