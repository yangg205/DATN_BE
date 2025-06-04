using Microsoft.AspNetCore.SignalR;
using DATNBACK_END.Services;
using DATNBACK_END.Models.modelTemp;
using DATNBACK_END.Models;

public class GameHub : Hub
{
    private readonly IPlayerCharacterSkillService _playerCharacterSkillService;
    private readonly IPlayerCharacterService _playerCharacterService;
    private readonly IPlayerService _playerService;

    public GameHub(IPlayerCharacterSkillService playerCharacterSkillService,
                   IPlayerCharacterService playerCharacterService,
                   IPlayerService playerService)
    {
        _playerCharacterService = playerCharacterService;
        _playerService = playerService;
        _playerCharacterSkillService = playerCharacterSkillService;
    }
    public async Task<ReturnPlayerCharacterSkill> UpdateSkill(int playerCharacterId, int skillTreeId)
    {
        return await _playerCharacterSkillService.UpdateSkill(playerCharacterId, skillTreeId);
    }
    public async Task<ReturnPlayer> RegisterPendingPlayer(string email, string password, string repassword)
    {
        return await _playerService.RegisterPendingPlayer(email, password, repassword);
    }
    public async Task<ReturnPlayer> VerifyOtpAndRegister(string email, string otp)
    {
        return await _playerService.VerifyOtpAndRegister(email, otp);
    }
    public async Task<ReturnPlayer> Login(string email, string password)
    {
        return await _playerService.Login(email, password);
    }
    public async Task<ReturnPlayerCharacter> SaveGame(Player_Characters playerCharacters)
    {
        return await _playerCharacterService.SaveGame(playerCharacters);
    }
    public async Task<ReturnPlayerCharacter> ContinueGame(int playerCharacterId)
    {
        return await _playerCharacterService.ContinueGame(playerCharacterId);
    }
    public async Task<ReturnPlayerCharacter> SelectCharacter(int playerId, int characterId)
    {
        return await _playerCharacterService.SelectCharacter(playerId, characterId);
    }
    public async Task<List<Ranking>> RankingPlayerCharacter(int characterId)
    {
        return await _playerCharacterService.RankingPlayerCharacter(characterId);
    }
    public async Task<PlayerCharacterSkillPoint> GetSkillPoint(int playerCharacterId)
    {
        return await _playerCharacterService.GetSkillPoint(playerCharacterId);
    }
    public async Task<bool> DeleteSkill(int playerCharacterId, int skillTreeId)
    {
        return await _playerCharacterSkillService.DeleteSkill(playerCharacterId, skillTreeId);
    }
    public async Task<Player> GetPlayerByEmail(string email)
    {
        return await _playerService.GetPlayersByEmail(email);
    }
}
