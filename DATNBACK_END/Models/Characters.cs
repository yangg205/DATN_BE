using System.ComponentModel.DataAnnotations;

namespace DATNBACK_END.Models;
    public class Characters{
        [Key]
        public int Character_id { get; set; }
        public string? Name {get;set;}
        public int? Hp { get; set; }
        public int? Exp { get; set; }
        public int? Stamina { get; set; }
        public int? Mana { get; set; }
        public int? Atk { get; set; }
        public int? Def { get; set; }
        public float? Crit { get; set; }
        public float? Critdmg { get; set; }
        public int? Magic {get;set;}
        public int? Skill1 {get;set;}
        public int? Skill2 {get;set;}
        public int? Skill3 {get;set;}
    }
