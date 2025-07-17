using System.ComponentModel.DataAnnotations;

namespace DATNBACK_END.Models;

public class Player_Achievement
{
    [Key]
    public int Player_Achievement_id { get; set; } 
    public int Player_Character_id { get; set; }   
    public int Achievement_id { get; set; }        
    public int Current_Amount { get; set; }       
    public bool Is_Completed { get; set; }         
    public DateTime? Completion_Date { get; set; }
}