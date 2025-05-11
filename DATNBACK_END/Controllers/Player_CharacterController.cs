using DATNBACK_END.Models;
using DATNBACK_END.Services;
using Microsoft.AspNetCore.Mvc;

namespace DATNBACK_END.Controllers;
[Controller]
public class Player_CharactersController : ControllerBase
{
    private readonly IPlayerCharacterService _playerCharacterService;
    private readonly IPlayerCharacterSkillService _playerCharacterSkillService;
    public Player_CharactersController(IPlayerCharacterService playerCharacterService, IPlayerCharacterSkillService playerCharacterSkillService)
    {
        _playerCharacterSkillService = playerCharacterSkillService;
        _playerCharacterService = playerCharacterService;
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
    public async Task<IActionResult> ContinueGame([FromBody] PlayerId_CharacterId playerId_CharacterId)
    {
        try
        {
            var result = await _playerCharacterService.ContinueGame(playerId_CharacterId.Player_id, playerId_CharacterId.Character_id);
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
    public async Task<IActionResult> UpdateSkill([FromQuery] int playerCharacterId, [FromQuery] int skillTreeId)
    {
        try
        {
            var result = await _playerCharacterSkillService.UpdateSkill(playerCharacterId, skillTreeId);
            return Ok(new { data = result });
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

}