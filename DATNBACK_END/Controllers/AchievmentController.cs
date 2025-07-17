using DATNBACK_END.Models.DTO;
using DATNBACK_END.Services.Achievment;
using Microsoft.AspNetCore.Mvc;

namespace DATNBACK_END.Controllers.AchievmentController;

[Route("achievements")]
[ApiController]
public class AchievmentController : ControllerBase
{
    private readonly IAchievmentService _achievmentService;

    public AchievmentController(IAchievmentService achievmentService)
    {
        _achievmentService = achievmentService;
    }

    [HttpGet("GetAllAchievements")]
    public async Task<IActionResult> GetAllAchievements([FromQuery] int playerCharacterId)
    {
        var achievements = await _achievmentService.GetPlayerAchievementsAsync(playerCharacterId);
        return Ok(achievements);
    }
    [HttpPost("complete")]
    public async Task<IActionResult> CompleteAchievement([FromBody] CompleteAchievementRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedAchievement = await _achievmentService.CompletePlayerAchievement(
            request.PlayerCharacterId,
            request.AchievementId
        );

        return Ok(new { Message = "Thành tựu đã được hoàn thành và trao thưởng.", Achievement = updatedAchievement });

    }
}