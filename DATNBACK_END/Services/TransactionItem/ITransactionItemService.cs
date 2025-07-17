using DATNBACK_END.Models.DTO.Transaction;

namespace DATNBACK_END.Services.TransactionItem;
public interface ITransactionService
{
    // Xử lý logic mua vật phẩm, cập nhật Player.Total_Money và Inventory
    Task<BuyItemResponseDTO> ProcessPurchaseAsync(BuyItemRequestDTO request);
}