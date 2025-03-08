using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InventoryServer.Entities;

[Table("FOSUser")]
public class User
{
    [Column("id")]
    public int? Id { get; set; }

    [Column("username")]
    required public string Username { get; set; }

    [JsonIgnore]
    [Column("salt")]
    public string? Salt { get; set; }

    [JsonIgnore]
    [Column("password")]
    public string? Password { get; set; }

    [JsonIgnore]
    public List<StockEntry>? StockEntries { get; set; }
}
