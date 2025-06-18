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
    public async Task<IActionResult> GetAllAchievements()
    {
        var achievements = await _achievmentService.GetAllAchievementsAsync();
        return Ok(achievements);
    }
}