using DATNBACK_END.Models;
using DATNBACK_END.Models.DTO.Auth;
using DATNBACK_END.Models.DTO.Transaction;
using DATNBACK_END.Models.modelTemp;
using DATNBACK_END.Services;
using Microsoft.AspNetCore.Mvc;

namespace DATNBACK_END.Controllers;

[ApiController]
[Route("player")]
public class PlayerContoller : ControllerBase
{
    private readonly IPlayerService _playerService;
    private readonly IPlayerCharacterService _playerCharacterService;

    public PlayerContoller(IPlayerService playerService,
                           IPlayerCharacterService playerCharacterService)
    {
        _playerService = playerService;
        _playerCharacterService = playerCharacterService;
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
    [HttpPut("reset-password")]
    public async Task<IActionResult> resetPass([FromBody] ResetPass resetPass)
    {
        try
        {
            var result = await _playerService.ResetPassword(resetPass.email, resetPass.passsword, resetPass.newpassword, resetPass.renewpassword);
            return Ok(new { data = result });
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    [HttpPut("Update-name")]
    public async Task<IActionResult> resetPass([FromBody] NewName newName)
    {
        try
        {
            var result = await _playerService.UpdateName(newName.email, newName.newname);
            return Ok(new { data = result });
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    
    [HttpGet]
    [Route("/GetPlayerCurrencies/{playerId}")]
    public async Task<IActionResult> GetPlayerCurrencies(int playerId)
    {
        try
        {
            var player = await _playerService.GetPlayerByIdAsync(playerId);
            if (player == null)
            {
                return NotFound(new { data = $"Player with ID {playerId} not found." });
            }

            // Tìm nhân vật chính của người chơi để lấy Total_coin
            // Giả định PlayerId == Player_CharacterId cho đơn giản hoặc bạn cần logic để chọn nhân vật mặc định
            // Hoặc client sẽ truyền PlayerCharacterId riêng biệt
            var playerCharacter = await _playerCharacterService.GetPlayerCharacterByIdAsync(playerId); 
            
            return Ok(new
            {
                data = new
                {
                    TotalMoney = player.Total_Money ?? 0,
                    TotalCoin = playerCharacter?.Total_coin ?? 0 // Total_coin có thể là 0 nếu không tìm thấy nhân vật hoặc là null
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetPlayerCurrencies for PlayerId {playerId}: {ex.Message}");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    [Route("/ExchangeCurrency")]
    public async Task<IActionResult> ExchangeCurrency([FromBody] ExchangeCurrencyRequestDTO request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var response = await _playerService.ExchangeCurrencyAsync(request);
            if (response.Success)
            {
                return Ok(new { data = response });
            }
            return BadRequest(new { data = response.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in ExchangeCurrency: {ex.Message}");
            return StatusCode(500, ex.Message);
        }
    }
}
