using DATNBACK_END.Models;
using DATNBACK_END.Models.modelTemp;
using Microsoft.EntityFrameworkCore;

namespace DATNBACK_END.Services;

public class PlayerCharacterSkillService : IPlayerCharacterSkillService
{
    private readonly AppDataContexts.AppDataContext _context;
    private readonly IPlayerCharacterService _playerCharacterService;
    public PlayerCharacterSkillService(AppDataContexts.AppDataContext context, IPlayerCharacterService playerCharacterService)
    {
        _playerCharacterService = playerCharacterService;
        _context = context;
    }
    public async Task<ReturnPlayerCharacterSkill> UpdateSkill(int Player_Characters_id, int skill_tree_id)
    {
        try
        {
            var PC = await _context.Player_Characters.FirstOrDefaultAsync(x => x.Player_Character_id == Player_Characters_id);
            if (PC == null)
            {
                return await Task.FromResult(new ReturnPlayerCharacterSkill
                {
                    status = false,
                    message = "Nhân vật không tồn tại"
                });
            }
            if (PC.Skill_Point < 1)
            {
                return await Task.FromResult(new ReturnPlayerCharacterSkill
                {
                    status = false,
                    message = "Không đủ điểm đề nâng cấp skill"
                });
            }
            var skill_tree = await _context.Skill_tree.FirstOrDefaultAsync(x => x.Skill_tree_id == skill_tree_id && x.Characters_id == PC.Characters_id);
            if (skill_tree == null)
            {
                return await Task.FromResult(new ReturnPlayerCharacterSkill
                {
                    status = false,
                    message = "Skill không tồn tại trong skill tree của nhân vật"
                });
            }
            // Kiểm tra skill cha (nếu có)
            if (skill_tree.Id_parent != null)
            {
                var parentSkillUnlocked = await _context.Player_Character_Skill
                    .AnyAsync(pcs => pcs.Player_Character_id == Player_Characters_id && pcs.Skill_tree_id == skill_tree.Id_parent);
                if (!parentSkillUnlocked)
                {
                    return await Task.FromResult(new ReturnPlayerCharacterSkill
                    {
                        status = false,
                        message = "Vui lòng mở khóa skill cha trước"
                    });
                }
            }

            // Kiểm tra xem skill đã được mở khóa chưa
            var isSkillUnlocked = await _context.Player_Character_Skill
                .AnyAsync(pcs => pcs.Player_Character_id == Player_Characters_id && pcs.Skill_tree_id == skill_tree_id);
            if (isSkillUnlocked)
            {
                return await Task.FromResult(new ReturnPlayerCharacterSkill
                {
                    status = false,
                    message = "Skill đã được mở khóa"
                });
            }

            // Giảm Skill_Point
            await _playerCharacterService.UpdateSkillPoint(Player_Characters_id, -1);

            // Thêm bản ghi vào Player_Character_Skill
            var playerSkill = new Player_Character_Skill
            {
                Player_Character_id = Player_Characters_id,
                Skill_tree_id = skill_tree_id
            };
            await _context.Player_Character_Skill.AddAsync(playerSkill);

            // Lưu thay đổi
            _context.Player_Characters.Update(PC);
            await _context.SaveChangesAsync();

            return await Task.FromResult(new ReturnPlayerCharacterSkill
            {
                status = true,
                message = "Nâng cấp skill thành công",
                Player_Character_Skill = playerSkill
            });
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            System.Console.WriteLine(e.InnerException);
            return await Task.FromResult(new ReturnPlayerCharacterSkill
            {
                status = false,
                message = "Lỗi hệ thống khi UPDATESKILL"
            });
        }
    }
    public async Task<bool> DeleteSkill(int Player_Characters_id, int skill_tree_id)
    {
        try
        {
            var playerCharacterSkill = await _context.Player_Character_Skill
                .FirstOrDefaultAsync(x => x.Player_Character_id == Player_Characters_id && x.Skill_tree_id == skill_tree_id);
            if (playerCharacterSkill == null)
            {
                return false; // Skill không tồn tại
            }

            _context.Player_Character_Skill.Remove(playerCharacterSkill);
            await _playerCharacterService.UpdateSkillPoint(Player_Characters_id, 1);
            await _context.SaveChangesAsync();

            return true; // Xóa thành công
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            System.Console.WriteLine(e.InnerException);
            return false; // Lỗi hệ thống khi DELETE SKILL
        }
    }
}