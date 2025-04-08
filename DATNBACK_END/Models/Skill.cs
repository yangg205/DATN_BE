using System.ComponentModel.DataAnnotations;

namespace DATNBACK_END.Models;
public class Skill
{
    [Key]
    public int Skill_id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? Atk { get; set; }
    public int? Def { get; set; }
    public int? Magic { get; set; }
    public int? HP { get; set;}
    public int? MP { get; set; }
    public float? Atk_speed { get; set; }
    public float? Crit { get; set; }
    public float? Critdame { get; set; }
}