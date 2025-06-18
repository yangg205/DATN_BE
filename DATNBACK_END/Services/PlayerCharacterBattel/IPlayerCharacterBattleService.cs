using DATNBACK_END.Models.modelTemp;

namespace DATNBACK_END.Services;
    public interface IPlayerCharacterBattleService
    {
        Task<Battle> Battle(int Player_Characters_id, int Boss_id, int Death, double maxtime, double realtime);
    }
