using System.ComponentModel.DataAnnotations;

namespace DATNBACK_END.Models;

public class TransactionItem
{
    [Key]
    public int TransactionItem_id { get; set; }
    public int Transaction_id { get; set; }
    public int Item_id { get; set; }
    public int Quantity { get; set; }
    public int UnitPrice { get; set; }
}