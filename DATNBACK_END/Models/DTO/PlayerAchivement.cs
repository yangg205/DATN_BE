namespace DATNBACK_END.Models.DTO;
// Trong thư mục DTOs hoặc Models/DTOs (tạo nếu chưa có)
public class PlayerAchievementDto
{
    public int AchievementId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int RequiredAmount { get; set; }
    public int RewardCoin { get; set; }
    public int RewardExp { get; set; }
    public int CurrentProgress { get; set; }
    public bool IsCompleted { get; set; }
    public string? Status { get; set; } // "Completed", "InProgress", "NotStarted"
    public string? IconPath { get; set; } // Đường dẫn đến biểu tượng của thành tựu
}
