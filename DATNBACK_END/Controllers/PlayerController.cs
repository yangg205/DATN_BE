using DATNBACK_END.Models;
using DATNBACK_END.Services;
using Microsoft.AspNetCore.Mvc;

namespace DATNBACK_END.Controllers;
[ApiController]
public class PlayerContoller : ControllerBase
{
    private readonly IPlayerService _playerService;
    public PlayerContoller(IPlayerService playerService)
    {
        _playerService = playerService;
    }
    [HttpPost]
    [Route("api/addPlayer")]
    public async Task<IActionResult> AddPlayer([FromBody] Player player)
    {
        try
        {
            var Result = await _playerService.AddPlayer(player.Email, player.Name, player.Password);
            return Ok(new{
                data = Result
            });
        }
        catch (Exception e)
        {
            return StatusCode(500,e.Message);
        }
    }
    [HttpGet]
    [Route("api/getPlayers")]
    public async Task<IActionResult> getPlayer(){
        try
        {
            var players = await _playerService.GetPlayers();
            return Ok(players);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
