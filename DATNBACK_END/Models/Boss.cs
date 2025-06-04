using System.ComponentModel.DataAnnotations;

namespace DATNBACK_END.Models;

public class Boss
{
    [Key]
    public int Boss_id { get; set; }
    public string? Name { get; set; }
    public int? Level { get; set; }
    public int? Hp { get; set; }
    public int? Dame { get; set; }
    public int? Def { get; set; }
    public int? Location_x { get; set; }
    public int? Location_y { get; set; }
    public int? Location_z { get; set; }
}
