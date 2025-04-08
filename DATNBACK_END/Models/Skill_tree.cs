using System.ComponentModel.DataAnnotations;

namespace DATNBACK_END.Models;
    public class Skill_tree{
        [Key]
        public int Skill_tree_id { get; set; }
        public int Player_Character_id { get; set; }
        public int Skill_id {get;set;}
        public int Id_parent { get; set; }
        public string? Name {get;set;}
        public string? Description {get;set;}
        public int? Hp {get;set;}
        public int? Atk {get;set;}
        public int? Def {get;set;}
        public int? Magic {get;set;}
        public float? Crit {get;set;}
        public float? Critdame {get;set;}
        public int? Mana {get;set;}
        public int? Maxlevel {get;set;}
        
    }