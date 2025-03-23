using System.ComponentModel.DataAnnotations;

namespace DATNBACK_END.Models;

public class Achievement
{
    [Key]
    public int Achievement_id { get; set; }
    public string? Name { get; set; }
    public int? Amount { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public int? Reward_Coin { get; set; }
    public int? Reward_Exp { get; set; }
}
