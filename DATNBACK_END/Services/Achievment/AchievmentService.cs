using DATNBACK_END.Models;
using DATNBACK_END.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DATNBACK_END.Services.Achievment;

public class AchievmentService : IAchievmentService
{
    private readonly AppDataContexts.AppDataContext _context;

    public AchievmentService(AppDataContexts.AppDataContext context)
    {
        _context = context;
    }

    // Phương thức mới để lấy tất cả thành tựu cùng với tiến độ của người chơi
    public async Task<List<PlayerAchievementDto>> GetPlayerAchievementsAsync(int playerCharacterId)
    {
        try
        {
            // Lấy tất cả các định nghĩa thành tựu
            var allAchievements = await _context.Achievement.ToListAsync();

            if (allAchievements == null || !allAchievements.Any())
            {
                throw new Exception("Không có thành tựu nào được định nghĩa.");
            }

            // Lấy tiến độ hiện tại của người chơi cho tất cả các thành tựu mà họ đã có thông tin
            var playerProgress = await _context.Player_Achievement
                .Where(pa => pa.Player_Character_id == playerCharacterId)
                .ToListAsync();

            var result = new List<PlayerAchievementDto>();

            foreach (var achievementDef in allAchievements)
            {
                // Tìm kiếm tiến độ của người chơi cho thành tựu này
                var playerAch = playerProgress.FirstOrDefault(pa => pa.Achievement_id == achievementDef.Achievement_id);

                var dto = new PlayerAchievementDto
                {
                    AchievementId = achievementDef.Achievement_id,
                    Name = achievementDef.Name,
                    Description = achievementDef.Description,
                    RequiredAmount = achievementDef.Amount, // Lấy từ Amount của bảng Achievement
                    RewardCoin = achievementDef.Reward_Coin,
                    RewardExp = achievementDef.Reward_Exp,
                    CurrentProgress = playerAch?.Current_Amount ?? 0, // Nếu chưa có bản ghi, tiến độ là 0
                    IsCompleted = playerAch?.Is_Completed ?? false, // Nếu chưa có bản ghi, chưa hoàn thành
                    IconPath = achievementDef.IconPath // Đường dẫn đến biểu tượng của thành tựu
                };

                // Xác định Status dựa trên IsCompleted và CurrentProgress
                if (dto.IsCompleted)
                {
                    dto.Status = "Completed";
                }
                else if (dto.CurrentProgress > 0)
                {
                    dto.Status = "InProgress";
                }
                else
                {
                    dto.Status = "NotStarted";
                }

                result.Add(dto);
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception("Lỗi khi lấy danh sách thành tựu của người chơi: " + ex.Message);
        }
    }
     // Trong DATNBACK_END.Services.Achievment.AchievmentService.cs
public async Task<PlayerAchievementDto> CompletePlayerAchievement(int playerCharacterId, int achievementId)
{
    using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {
        // 1. Lấy định nghĩa thành tựu
        var achievementDef = await _context.Achievement
            .FirstOrDefaultAsync(a => a.Achievement_id == achievementId);

        if (achievementDef == null)
        {
            throw new Exception($"Không tìm thấy thành tựu với ID: {achievementId}");
        }

        // 2. Tìm hoặc tạo bản ghi tiến độ của người chơi cho thành tựu này
        var playerAchievement = await _context.Player_Achievement
            .FirstOrDefaultAsync(pa => pa.Player_Character_id == playerCharacterId &&
                                       pa.Achievement_id == achievementId);

        if (playerAchievement == null)
        {
            playerAchievement = new Player_Achievement
            {
                Player_Character_id = playerCharacterId,
                Achievement_id = achievementId,
                Current_Amount = 0,
                Is_Completed = false,
                Completion_Date = null
            };
            _context.Player_Achievement.Add(playerAchievement);
        }

        // 3. Kiểm tra xem thành tựu đã hoàn thành trên server chưa
        if (playerAchievement.Is_Completed)
        {
            // Thành tựu đã hoàn thành trước đó, không cần làm gì
            Console.WriteLine($"Achievement {achievementId} for player {playerCharacterId} already completed on server.");
        }
        else
        {
            // Cập nhật trạng thái hoàn thành trên server
            playerAchievement.Is_Completed = true;
            playerAchievement.Completion_Date = DateTime.UtcNow;

            // --- LOGIC TRAO THƯỞNG ---

            // Bước 1: Tìm Player_Characters (không cần Include ở đây)
            var playerCharacter = await _context.Player_Characters
                .FirstOrDefaultAsync(pc => pc.Player_Character_id == playerCharacterId);

            if (playerCharacter == null)
            {
                throw new Exception($"Không tìm thấy nhân vật người chơi với ID: {playerCharacterId}");
            }

            // Bước 2: Cập nhật Total_coin và Current_exp của Player_Characters
            playerCharacter.Total_coin += achievementDef.Reward_Coin;
            playerCharacter.Current_exp += achievementDef.Reward_Exp;

            // Bước 3: Tìm Player (nếu bạn cần cập nhật Total_Money ở Player model)
            // LƯU Ý: Để dòng này hoạt động, bạn cần đảm bảo Player_Characters model có thuộc tính Player_id.
            // Và nếu Total_Money nằm trong Player model, bạn cần truy vấn nó.
            var player = await _context.Player
                .FirstOrDefaultAsync(p => p.Player_id == playerCharacter.Player_id); // Dùng Player_id từ playerCharacter

            if (player == null)
            {
                // Điều này có thể xảy ra nếu Player_id trong Player_Characters không hợp lệ,
                // hoặc không có Player nào với ID đó. Bạn nên xử lý lỗi hoặc log ở đây.
                Console.WriteLine($"Warning: Player with ID {playerCharacter.Player_id} not found for PlayerCharacter {playerCharacterId}. Total_Money could not be updated.");
            }
            else
            {
                // Cập nhật Total_Money của Player (nếu có)
                player.Total_Money += achievementDef.Reward_Coin; // Giả sử Reward_Coin cũng thêm vào Total_Money
            }

            await _context.SaveChangesAsync();
            Console.WriteLine($"Player {playerCharacterId} completed achievement {achievementId} and received rewards.");
        }

        await transaction.CommitAsync();

        // Trả về DTO của thành tựu đã cập nhật
        var updatedDto = new PlayerAchievementDto
        {
            AchievementId = achievementDef.Achievement_id,
            Name = achievementDef.Name,
            Description = achievementDef.Description,
            RequiredAmount = achievementDef.Amount,
            RewardCoin = achievementDef.Reward_Coin,
            RewardExp = achievementDef.Reward_Exp,
            CurrentProgress = playerAchievement.Current_Amount,
            IsCompleted = playerAchievement.Is_Completed
        };
        updatedDto.Status = updatedDto.IsCompleted ? "Completed" : "InProgress";

        return updatedDto;
    }
    catch (Exception ex)
    {
        await transaction.RollbackAsync();
        Console.Error.WriteLine($"Error completing achievement for player {playerCharacterId}, achievement {achievementId}: {ex.Message}");
        throw new Exception($"Lỗi khi hoàn thành thành tựu: {ex.Message}", ex);
    }
}
}