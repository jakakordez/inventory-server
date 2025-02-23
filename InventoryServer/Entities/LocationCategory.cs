using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryServer.Entities;

[Table("StorageLocationCategory")]
public class LocationCategory
{
    [Column("id")]
    required public int Id { get; set; }

    [Column("parent_id")]
    required public int? ParentId { get; set; }

    [Column("name")]
    required public string Name { get; set; }

    required public ICollection<Location> Locations { get; set; }


    [NotMapped]
    public List<LocationCategory>? Subcategories { get; set; }
}
