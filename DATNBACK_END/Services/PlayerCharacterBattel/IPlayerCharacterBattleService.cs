using DATNBACK_END.Models;
using DATNBACK_END.Models.modelTemp;

namespace DATNBACK_END.Services;

public interface IPlayerCharacterBattleService
{
    Task<Battle> Battle(int Player_Characters_id, int Boss_id, int Death, double maxtime, double realtime);
    Task<Boss> createBoss(int Boss_id, string Name, int Level, int Hp, int Dame, int Def, int Location_x, int Location_y, int Location_z, double Difficult);
    Task<List<Boss>> GetAllBoss();
}
