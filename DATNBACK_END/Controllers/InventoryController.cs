using DATNBACK_END.Models.DTO.Transaction;
using DATNBACK_END.Services;
using Microsoft.AspNetCore.Mvc;

namespace DATNBACK_END.Controllers;
[Controller] // Sử dụng [Controller] theo phong cách của bạn
public class InventoryController : ControllerBase
{
    private readonly IPlayerCharacterService _playerCharacterService;

    public InventoryController(IPlayerCharacterService playerCharacterService)
    {
        _playerCharacterService = playerCharacterService;
    }

    [HttpGet]
    [Route("/GetPlayerInventory/{playerCharacterId}")] 
    public async Task<IActionResult> GetPlayerInventory(int playerCharacterId)
    {
        try
        {
            var playerCharacter = await _playerCharacterService.GetPlayerCharacterByIdAsync(playerCharacterId);
            if (playerCharacter == null)
            {
                return NotFound(new { data = $"Player Character with ID {playerCharacterId} not found." }); // Trả về lỗi theo format
            }

            var inventory = await _playerCharacterService.GetPlayerInventoryAsync(playerCharacterId);
            return Ok(new { data = inventory }); // Trả về theo format { data = result }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetPlayerInventory for PlayerCharacterId {playerCharacterId}: {ex.Message}");
            return StatusCode(500, ex.Message);
        }
    }
    [HttpPost]
    [Route("/ConsumeItem")]
    public async Task<IActionResult> ConsumeItem([FromBody] ConsumeItemRequestDTO request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var success = await _playerCharacterService.ConsumeItemAsync(request);
            if (success)
            {
                return Ok(new { data = true }); // Trả về theo format { data = true }
            }
            return BadRequest(new { data = false, message = "Consumption failed: Not enough items or other issue." }); // Trả về lỗi theo format
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in ConsumeItem: {ex.Message}");
            return StatusCode(500, ex.Message);
        }
    }
}