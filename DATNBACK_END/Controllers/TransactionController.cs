using DATNBACK_END.Models.DTO.Transaction;
using DATNBACK_END.Services.TransactionItem;
using Microsoft.AspNetCore.Mvc;

namespace DATNBACK_END.Controllers;

[Controller] // Sử dụng [Controller] theo phong cách của bạn
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost]
    [Route("/BuyItem")]
    public async Task<IActionResult> BuyItem([FromBody] BuyItemRequestDTO request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // Trả về lỗi validate input nếu có
        }

        try
        {
            var response = await _transactionService.ProcessPurchaseAsync(request);
            if (response.Success)
            {
                return Ok(new { data = response }); // Trả về theo format { data = result }
            }
            return BadRequest(new { data = response.Message }); // Trả về lỗi theo format
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in BuyItem: {ex.Message}");
            return StatusCode(500, ex.Message);
        }
    }
}