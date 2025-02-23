using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InventoryServer.Entities;

[Table("Part")]
public class Part
{
    [Column("id")]
    public int? Id { get; set; }

    [Column("category_id")]
    required public int CategoryId { get; set; }

    [Column("name")]
    required public string Name { get; set; }

    [Column("description")]
    required public string Description { get; set; }

    [Column("stockLevel")]
    public int StockLevel { get; set; }

    [Column("storageLocation_id")]
    required public int LocationId { get; set; }

    [NotMapped]
    public DateTime? LastEntry => Entries?.Max(e => e.Timestamp);

    [JsonIgnore]
    public PartCategory? Category { get; set; }

    [JsonIgnore]
    public Location? Location { get; set; }

    [JsonIgnore]
    public List<StockEntry>? Entries { get; set; }
}
