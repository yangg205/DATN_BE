using DATNBACK_END.Models;
using DATNBACK_END.Models.modelTemp;
using DATNBACK_END.Services;
using Microsoft.AspNetCore.Mvc;

namespace DATNBACK_END.Controllers;

[Controller]
public class Player_CharactersController : ControllerBase
{
    private readonly IPlayerCharacterService _playerCharacterService;
    private readonly IPlayerCharacterSkillService _playerCharacterSkillService;
    private readonly IPlayerCharacterBattleService _playerCharacterBattleService;
    public Player_CharactersController(IPlayerCharacterService playerCharacterService, IPlayerCharacterSkillService playerCharacterSkillService, IPlayerCharacterBattleService playerCharacterBattleService)
    {
        _playerCharacterSkillService = playerCharacterSkillService;
        _playerCharacterService = playerCharacterService;
        _playerCharacterBattleService = playerCharacterBattleService;
    }
    [HttpPost]
    [Route("/SaveGame")]
    public async Task<IActionResult> SaveGame([FromBody] Player_Characters player_Characters)
    {
        try
        {
            var result = await _playerCharacterService.SaveGame(player_Characters);
            return Ok(new
            {
                data = result
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    [HttpPost]
    [Route("/ContinueGame")]
    public async Task<IActionResult> ContinueGame([FromQuery] int playerId, int characterId)
    {
        try
        {
            var result = await _playerCharacterService.ContinueGame(playerId,characterId);
            return Ok(new
            {
                data = result
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    [HttpPost]
    [Route("/SelecteCharacter")]
    public async Task<IActionResult> SelecteCharacter([FromBody] PlayerId_CharacterId playerId_CharacterId)
    {
        try
        {
            var result = await _playerCharacterService.SelectCharacter(playerId_CharacterId.Player_id, playerId_CharacterId.Character_id);
            return Ok(new
            {
                data = result
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    [HttpDelete]
    [Route("/DeleteAllPlayerCharacter")]
    public async Task<IActionResult> DeleteAllPlayerCharacter()
    {
        try
        {
            var result = await _playerCharacterService.DeleteAllPlayerCharacter();
            return Ok(new
            {
                data = result
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    [HttpGet]
    [Route("/GetAllPlayerCharacter")]
    public async Task<IActionResult> GetAllPlayerCharacter()
    {
        try
        {
            var result = await _playerCharacterService.GetAllPlayerCharacter();
            return Ok(new
            {
                data = result
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    [HttpPost]
    [Route("UpdateSkill")]
    public async Task<IActionResult> UpdateSkill([FromBody] UpdateSkill updateSkill)
    {
        try
        {
            var result = await _playerCharacterSkillService.UpdateSkill(updateSkill.PlayerCharacterId, updateSkill.SkillTreeId);
            return Ok(new { data = result });
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    [HttpPost]
    [Route("UpdateSkillPoint")]
    public async Task<IActionResult> UpdateSkillPoint([FromBody] PLayerCharacterUpdatePoint playerCharacter)
    {
        try
        {
            var result = await _playerCharacterService.UpdateSkillPoint(playerCharacter.PlayerCharacterId, playerCharacter.Point);
            return Ok(new { data = result });
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    [HttpGet]
    [Route("RankingPlayerCharacter/{Character_id}")]
    public async Task<IActionResult> RankingPlayerCharacter(int Character_id)
    {
        try
        {
            var result = await _playerCharacterService.RankingPlayerCharacter(Character_id);
            return Ok(new { data = result });
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    [HttpGet]
    [Route("GetSkillPoint/{Player_Character_id}")]
    public async Task<IActionResult> GetSkillPoint(int Player_Character_id)
    {
        try
        {
            var result = await _playerCharacterService.GetSkillPoint(Player_Character_id);
            return Ok(new { data = result });
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    [HttpDelete]
    [Route("DeleteSkill/{Player_Characters_id}/{skill_tree_id}")]
    public async Task<IActionResult> DeleteSkill(int Player_Characters_id, int skill_tree_id)
    {
        try
        {
            var result = await _playerCharacterSkillService.DeleteSkill(Player_Characters_id, skill_tree_id);
            return Ok(new { data = result });
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    [HttpPost]
    [Route("Battle")]
    public async Task<IActionResult> Battle([FromBody] BattleRq battleRequest)
    {
        try
        {
            var result = await _playerCharacterBattleService.Battle(battleRequest.Player_Characters_id, battleRequest.Boss_id, battleRequest.Death, battleRequest.maxtime, battleRequest.realtime);
            return Ok(new { data = result });
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    [HttpGet]
    [Route("SelectedCharacter/{Player_id}/{Character_id}")]
    public async Task<IActionResult> SelectedCharacter(int Player_id, int Character_id)
    {
        try
        {
            var result = await _playerCharacterService.SelectedCharacter(Player_id, Character_id);
            return Ok(new { data = result });
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}