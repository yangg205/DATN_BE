using System.ComponentModel.DataAnnotations;

namespace DATNBACK_END.Models;

public class Transactions
{
    [Key]
    public int Transaction_id { get; set; }
    public DateTime? Trandate { get; set; } = DateTime.Now;
    public decimal? Price { get; set; }
    public int? TotalAmount { get; set; }
    public int Player_CharacterId { get; set; }
}
