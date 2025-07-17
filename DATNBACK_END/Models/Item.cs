using System.ComponentModel.DataAnnotations;
namespace DATNBACK_END.Models;

public class Item
{
    [Key]
    public int Item_id { get; set; }
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? IconPath { get; set; }
}
