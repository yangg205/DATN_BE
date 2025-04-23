using System.ComponentModel.DataAnnotations;

namespace DATNBACK_END.Models;

public class Skill
{
    [Key]
    public int Skill_id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public double? Atk { get; set; }
    public double? Def { get; set; }
    public double? Magic { get; set; }
    public int? HP { get; set; }
    public int? MP { get; set; }
    public double? Atk_speed { get; set; }
    public double? Crit { get; set; }
    public double? Critdame { get; set; }
}
