using System.ComponentModel.DataAnnotations;

namespace DATNBACK_END.Models;

public class Inventory
{
    [Key]
    public int Inventory_id { get; set; }
    public int Player_CharacterId { get; set; }
    public int Item_id { get; set; }
    public int Quantity { get; set; }

}
