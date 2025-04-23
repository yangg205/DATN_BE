using DATNBACK_END.Models;
using DATNBACK_END.Models.modelTemp;

namespace DATNBACK_END.Services;
public interface IPlayerCharacterService{
    Task<ReturnPlayerCharacter> SaveGame(Player_Characters player_Characters);
    Task<ReturnPlayerCharacter> ContinueGame(int Player_id, int Character_id);
    Task<ReturnPlayerCharacter> SelectCharacter(int Player_id, int Character_id);
    Task<bool> DeleteAllPlayerCharacter();
    Task<List<Player_Characters>> GetAllPlayerCharacter();
}