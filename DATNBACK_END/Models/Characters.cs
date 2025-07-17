using System.ComponentModel.DataAnnotations;

namespace DATNBACK_END.Models;

public class Characters
{
    [Key]
    public int Characters_id { get; set; }
    public string? Name { get; set; }
    public int? Hp { get; set; }
    public int? Stamina { get; set; }
    public int? Mana { get; set; }
    public double? Atk { get; set; }
    public double? Def { get; set; }
    public double? Crit { get; set; }
    public double? Critdmg { get; set; }
    public double? Magic { get; set; }
    public double? AtkSpeed {get;set;}
}
