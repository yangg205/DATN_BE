using DATNBACK_END.Models;

namespace DATNBACK_END.Services.Achievment;
public interface IAchievmentService
{
    Task<List<Achievement>> GetAllAchievementsAsync();
}