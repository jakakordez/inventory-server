using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InventoryServer.Entities;

[Table("StockEntry")]
public class StockEntry
{
    [Column("id")]
    public int? Id { get; set; }

    [Column("part_id")]
    required public int? PartId {  get; set; }

    [Column("user_id")]
    required public int? UserId { get; set; }

    [Column("stockLevel")] 
    required public int Change {  get; set; }

    [Column("dateTime")]
    required public DateTime Timestamp { get; set; }

    [Column("comment")]
    public string? Comment { get; set; }

    [JsonIgnore]
    public Part? Part { get; set; }

    public User? User { get; set; }
}
