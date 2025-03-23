using System.ComponentModel.DataAnnotations;

namespace DATNBACK_END.Models;
    public class Player_Character_Battle
    {
        [Key]
        public int Player_battle_id { get; set; }
        public int Player_Character_id { get; set; }
        public int Battle_id { get; set; }
        public int? Death { get; set; }
        public int? Point { get; set; }
    }
