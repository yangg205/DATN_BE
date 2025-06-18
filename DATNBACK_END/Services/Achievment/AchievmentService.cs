using DATNBACK_END.Models;
using Microsoft.EntityFrameworkCore;

namespace DATNBACK_END.Services.Achievment;
public class AchievmentService : IAchievmentService
{
    private readonly AppDataContexts.AppDataContext _context;

    public AchievmentService(AppDataContexts.AppDataContext context)
    {
        _context = context;
    }

    public async Task<List<Achievement>> GetAllAchievementsAsync()
    {
        try
        {
            var achievements = await _context.Achievement.ToListAsync();
            if (achievements == null || achievements.Count == 0)
            {
                throw new Exception("Không có thành tựu nào.");
            }
            return achievements;
        }
        catch (Exception ex)
        {
            throw new Exception("Lỗi khi xuất thành tựu " + ex.Message);
        }
    }
}