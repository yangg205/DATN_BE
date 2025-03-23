using System.ComponentModel.DataAnnotations;

namespace DATNBACK_END.Models;
    public class Transactions
    {
        [Key]
        public int Transaction_id { get; set; }
        public DateTime? Trandate { get; set; } = DateTime.Now;
        public decimal? Price { get; set; }
        public int? Amount { get; set; }
        public int Item_id { get; set; }
        public int Player_Character_id { get; set; }
    }
