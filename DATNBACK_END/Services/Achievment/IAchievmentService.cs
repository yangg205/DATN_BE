using DATNBACK_END.Models;
using DATNBACK_END.Models.DTO;

namespace DATNBACK_END.Services.Achievment;

public interface IAchievmentService
{
    Task<List<PlayerAchievementDto>> GetPlayerAchievementsAsync(int playerCharacterId);
    Task<PlayerAchievementDto> CompletePlayerAchievement(int playerCharacterId, int achievementId);
}