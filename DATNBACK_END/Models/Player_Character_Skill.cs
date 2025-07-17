using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DATNBACK_END.Models
{
    public class Player_Character_Skill
    {
        [Key]
        public int Player_Skill_id { get; set; }
        public int Player_Character_id { get; set; }
        public int Skill_tree_id { get; set; }
    }
}