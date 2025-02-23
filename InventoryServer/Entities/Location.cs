using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InventoryServer.Entities;

[Table("StorageLocation")]
public class Location
{
    [Column("id")]
    required public int Id { get; set; }

    [Column("category_id")]
    required public int? CategoryId {  get; set; }

    [Column("name")]
    required public string Name { get; init; }

    [JsonIgnore]
    public LocationCategory? Category { get; set; }

    [JsonIgnore]
    public List<Part>? Parts {  get; set; }
}
