using System.ComponentModel.DataAnnotations;

namespace DATNBACK_END.Models;
public class Battle
{
    [Key]
    public int Battle_id { get; set; }
    public int Boss_id { get; set; }
}
