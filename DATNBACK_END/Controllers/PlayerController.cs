using DATNBACK_END.Models;
using DATNBACK_END.Services;
using Microsoft.AspNetCore.Mvc;

namespace DATNBACK_END.Controllers;
[ApiController]
[Route("api/player")]
public class PlayerContoller : ControllerBase
{
    private readonly IPlayerService _playerService;
    public PlayerContoller(IPlayerService playerService)
    {
        _playerService = playerService;
    }
    [HttpPost("register")]
    public async Task<IActionResult> RegisterTemp([FromBody] Dangky player)
    {
        try
        {
            var Result = await _playerService.RegisterPendingPlayer(player.Email,player.Password,player.Repassword);
            return Ok(new{
                data = Result
            });
        }
        catch (Exception e)
        {
            return StatusCode(500,e.Message);
        }
    }
    [HttpPost("verifyOtp{email},{otp}")]
    public async Task<IActionResult> VerifyOtp(string email,string otp){
        try{
            var Result = _playerService.VerifyOtpAndRegister(email,otp);
            return Ok(new{
               data = Result, 
            });
        }
        catch(Exception e){
            return StatusCode(500,e.Message);
        }
    }
    [HttpGet("getPlayers")]
    public async Task<IActionResult> getPlayer(){
        try
        {
            var players = await _playerService.GetPlayers();
            return Ok(players);
        }
        catch (Exception e)
        {
            return StatusCode(500,e.Message);
        }
    }
}
