using InventoryServer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InventoryServer.Controllers;

[Authorize]
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

    [HttpPost()]
    public async Task<IActionResult> Add([FromBody] Part part)
    {
        part.Id = null;
        part.StockLevel = 0;

        db.Parts.Add(part);
        await db.SaveChangesAsync();

        return Ok(part);
    }

    [HttpGet("{id}/history")]
    public async Task<IActionResult> GetEntries(
        [FromRoute] int id)
    {
        var list = await db.StockEntries
            .Include(e => e.User)
            .Where(p => p.PartId == id)
            .ToListAsync();
        return Ok(list);
    }

    [HttpPut("{id}/history")]
    public async Task<IActionResult> UpdateStock(
        [FromRoute] int id, 
        [FromBody] StockChange change)
    {
        if (change.NewLevel < 0)
        {
            return BadRequest("New level must be non-negative!");
        }
        using var transaction = await db.Database.BeginTransactionAsync();

        var part = await db.Parts.FindAsync(id);
        if (part == null)
        {
            return NotFound($"Part with id {id} not found");
        }
        int levelChange = change.NewLevel - part.StockLevel;

        var userId = User.Claims
            .Where(c => c.Type == ClaimTypes.NameIdentifier)
            .Select(c => (int?)Convert.ToInt32(c.Value))
            .FirstOrDefault();

        if (userId == null)
        {
            return BadRequest("User not found");
        }

        part.StockLevel = change.NewLevel;
        db.StockEntries.Add(new StockEntry()
        {
            PartId = id,
            Change = levelChange,
            Comment = change.Comment,
            Timestamp = DateTime.Now,
            UserId = userId
        });
        db.Parts.Update(part);
        await db.SaveChangesAsync();

        await transaction.CommitAsync();

        return Ok();
    }

    [HttpGet("unchecked")]
    public async Task<IActionResult> GetUncheckedParts()
    {
        var year = DateTime.Now.Year;
        if (HttpContext.Request.Query.TryGetValue("year", out var yearValue))
        {
            year = Convert.ToInt32(yearValue.ToString());
        }

        var parts = await db.Parts
            .Include(p => p.Entries)
            .Where(p => !p.Entries.Any(e => e.Timestamp.Year == year) && p.StockLevel > 0)
            .ToListAsync();

        return Ok(parts);
    }

    [HttpGet("report")]
    public async Task<IActionResult> GetReport()
    {
        var year = DateTime.Now.Year;
        if (HttpContext.Request.Query.TryGetValue("year", out var yearValue))
        {
            year = Convert.ToInt32(yearValue.ToString());
        }

        var changes = await db.StockEntries
            .Include(e => e.Part)
                .ThenInclude(p => p.Category)
            .Include(e => e.Part)
                .ThenInclude(p => p.Location)
            .Where(e => e.Timestamp.Year == year)
            .GroupBy(e => e.Part)
            .ToListAsync();

        var sum = changes
            .Select(c => new ChangeSum(c.Key, c.Sum(ch => ch.Change)))
            .Where(c => c.Change != 0);

        var exporter = new TableExporter<ChangeSum>(sum);
        exporter.AddColumn("ID", p => p.Part.Id, 10);
        exporter.AddColumn("Name", p => p.Part.Name, 50);
        exporter.AddColumn("Category", p => p.Part.Category.CategoryPath, 80);
        exporter.AddColumn("Location", p => p.Part.Location.Name, 50);
        exporter.AddColumn("Change", p => p.Change, 10);
        exporter.AddColumn("Stock level", p => p.Part.StockLevel, 10);
        return File(exporter.ExportToExcel(), "application/vnd.ms-excel");
    }

    private record ChangeSum(Part Part, int Change);

    public class StockChange
    {
        required public int NewLevel { get; init; }
        public string? Comment { get; init; }
    }
}
