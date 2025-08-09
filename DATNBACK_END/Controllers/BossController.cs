using DATNBACK_END.Models;
using DATNBACK_END.Services;
using Microsoft.AspNetCore.Mvc;

namespace DATNBACK_END.Controllers;
public class BossController : Controller
{
    private readonly IPlayerCharacterBattleService _playerCharacterBattleService;

    public BossController(IPlayerCharacterBattleService playerCharacterBattleService)
    {
        _playerCharacterBattleService = playerCharacterBattleService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateBoss([FromBody] Boss boss)
    {
        if (boss == null || string.IsNullOrEmpty(boss.Name) || boss.Level <= 0)
        {
            return BadRequest("Invalid boss data.");
        }

        try
        {
            var createdBoss = await _playerCharacterBattleService.createBoss(
                boss.Boss_id, 
                boss.Name, 
                boss.Level ?? 0, 
                boss.Hp ?? 0, 
                boss.Dame ?? 0, 
                boss.Def ?? 0, 
                boss.Location_x ?? 0, 
                boss.Location_y ?? 0, 
                boss.Location_z ?? 0, 
                boss.Difficult ?? 1.0
            );

            return Ok(createdBoss);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllBosses()
    {
        try
        {
            var bosses = await _playerCharacterBattleService.GetAllBoss();
            return Ok(bosses);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}