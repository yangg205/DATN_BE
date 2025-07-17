using DATNBACK_END.Models;
using DATNBACK_END.Models.DTO.Transaction;
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



    // Lấy Player_Characters Model
    Task<Player_Characters?> GetPlayerCharacterByIdAsync(int playerCharacterId);

    // Cập nhật số tiền Total_Coin của Player_Characters
    Task<bool> UpdatePlayerTotalCoinAsync(int playerCharacterId, decimal amountChange);

    // --- Các phương thức quản lý kho đồ người chơi (Inventory) ---
    // Thêm hoặc cập nhật vật phẩm vào kho đồ (cho mua hoặc nhận)
    Task<bool> AddOrUpdateInventoryItemAsync(int playerCharacterId, int itemId, int quantityChange);
    // Tiêu thụ vật phẩm khỏi kho đồ
    Task<bool> ConsumeItemAsync(ConsumeItemRequestDTO request);
    // Lấy toàn bộ kho đồ hiện tại của người chơi
    Task<List<PlayerInventoryItemDTO>> GetPlayerInventoryAsync(int playerCharacterId);
        // Cập nhật số tiền Total_Money của Player
    Task<bool> UpdatePlayerTotalMoneyAsync(int PlayerCharacterId, decimal amountChange);
}