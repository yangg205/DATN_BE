using DATNBACK_END.Models;
using DATNBACK_END.Models.modelTemp;

namespace DATNBACK_END.Services;

public interface IPlayerCharacterService
{
    Task<ReturnPlayerCharacter> SaveGame(Player_Characters player_Characters);
    Task<ReturnPlayerCharacter> ContinueGame(int playerId, int characterId);
    Task<ReturnPlayerCharacter> SelectCharacter(int Player_id, int Character_id);
    Task<bool> DeleteAllPlayerCharacter();
    Task<List<Player_Characters>> GetAllPlayerCharacter();
    Task<bool> UpdateSkillPoint(int playerCharacterId, int Point);
    Task<List<Ranking>> RankingPlayerCharacter(int Character_id);
    Task<PlayerCharacterSkillPoint> GetSkillPoint(int Player_Character_id);
    Task<ReturnPlayerCharacter> SelectedCharacter(int Player_id, int Character_id);

}