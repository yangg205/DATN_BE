using DATNBACK_END.Models;
using DATNBACK_END.Models.modelTemp;
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
            var Result = await _playerService.RegisterPendingPlayer(
                player.Email,
                player.Password,
                player.Repassword
            );
            return Ok(new { data = Result });
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("verifyOtp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOTP verifyOTP)
    {
        try
        {
            var Result = await _playerService.VerifyOtpAndRegister(verifyOTP.Email, verifyOTP.OTP);
            return Ok(new { data = Result });
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] DangNhap dangNhap)
    {
        try
        {
            var Result = await _playerService.Login(dangNhap.Email, dangNhap.Password);
            return Ok(new { data = Result });
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("getPlayers{email}")]
    public async Task<IActionResult> getPlayer(string email)
    {
        try
        {
            var players = await _playerService.GetPlayersByEmail(email);
            return Ok(players);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("InputTokenGG")]
    public async Task<IActionResult> LoginGG(string Token)
    {
        try
        {
            var result = _playerService.LoginWithGoogleToken(Token);
            return Ok(new { data = result });
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    [HttpDelete("deletePlayer{email}")]
    public async Task<IActionResult> deletePlayer(string email)
    {
        try
        {
            var result = await _playerService.deletePlayer(email);
            return Ok(new { data = result });
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}
