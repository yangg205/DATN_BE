using System.ComponentModel.DataAnnotations;

namespace DATNBACK_END.Models;
    public class Player_Characters
    {
        [Key]
        public int Player_Character_id { get; set; }
        public int Player_id { get; set; }
        public int Character_id { get; set; }
        public DateTime? Ownershipdate { get; set; } = DateTime.Now;
        public int? Current_hp {get;set;}
        public int? Current_exp {get;set;}
        public int? level {get;set;}
        public int? Skill_Point {get;set;}
        public float? Position_x {get;set;}
        public float? Position_y {get;set;}
        public float? Position_z {get;set;}
    }