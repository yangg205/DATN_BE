namespace DATNBACK_END.Models;

public class PendingPlayer
{
    public int PendingPlayer_id { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Password_Salt { get; set; }
    public string? OTP { get; set; }
    public string? Name { get; set; }
    public decimal? Total_Money { get; set; } = 0.00M;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
