using System.ComponentModel.DataAnnotations;

namespace DATNBACK_END.Models;

public class Player_Characters
{
    [Key]
    public int Player_Character_id { get; set; }
    public int Player_id { get; set; }
    public int Characters_id { get; set; }
    public DateTime? Ownershipdate { get; set; } = DateTime.Now;
    public int? Current_hp { get; set; }
    public int? Current_exp { get; set; }
    public int? level { get; set; }
    public int? Total_coin { get; set; }
    public int? Skill_Point { get; set; }
    public double? Position_x { get; set; }
    public double? Position_y { get; set; }
    public double? Position_z { get; set; }
    public DateTime Datesave { get; set; } = DateTime.Now;
}
