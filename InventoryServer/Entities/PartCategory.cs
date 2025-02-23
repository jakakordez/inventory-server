using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InventoryServer.Entities;

[Table("PartCategory")]
public class PartCategory
{
    [Column("id")]
    required public int Id { get; set; }

    [Column("parent_id")]
    required public int ParentId { get; set; }

    [Column("name")]
    required public string Name { get; set; }

    [JsonIgnore]
    public List<Part>? Parts { get; set; }
}
