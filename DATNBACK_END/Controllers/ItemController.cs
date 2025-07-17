
namespace DATNBACK_END.Controllers;
using Microsoft.AspNetCore.Mvc;
using DATNBACK_END.Services;
using DATNBACK_END.Models; // Để trả về Item Model trực tiếp
using System.Collections.Generic;
using System.Threading.Tasks;
using DATNBACK_END.Services.Item;

[Controller] 
public class ItemController : ControllerBase
{
    private readonly IItemService _itemService;

    public ItemController(IItemService itemService)
    {
        _itemService = itemService;
    }

    [HttpGet]
    [Route("/GetAllShopItems")] 
    public async Task<IActionResult> GetAllShopItems() 
    {
        try
        {
            var items = await _itemService.GetAllItemsAsync();
            return Ok(new { data = items }); 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetAllShopItems: {ex.Message}"); 
            return StatusCode(500, ex.Message);
        }
    }
}
