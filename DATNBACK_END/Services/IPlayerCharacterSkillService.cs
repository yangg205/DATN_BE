using DATNBACK_END.Models.modelTemp;
namespace DATNBACK_END.Services;

public interface IPlayerCharacterSkillService
{
    Task<ReturnPlayerCharacterSkill> UpdateSkill(int Player_Characters_id,int skill_tree_id);
}