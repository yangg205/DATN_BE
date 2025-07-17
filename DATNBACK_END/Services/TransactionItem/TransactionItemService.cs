namespace DATNBACK_END.Services.TransactionItem;
using DATNBACK_END.Models;
using DATNBACK_END.Models.DTO.Transaction;
using DATNBACK_END.Services.Item;
public class TransactionService : ITransactionService
{
    private readonly AppDataContexts.AppDataContext _context;
    private readonly IPlayerCharacterService _playerCharacterService; // Dùng để cập nhật Inventory
    private readonly IItemService _itemService;

    public TransactionService(AppDataContexts.AppDataContext context, IPlayerService playerService, IPlayerCharacterService playerCharacterService, IItemService itemService)
    {
        _context = context;
        _playerCharacterService = playerCharacterService;
        _itemService = itemService;
    }

    public async Task<BuyItemResponseDTO> ProcessPurchaseAsync(BuyItemRequestDTO request)
    {
        // Sử dụng transaction để đảm bảo tính nguyên tử của giao dịch (atomic)
        // Nếu có lỗi, mọi thay đổi sẽ được rollback
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var playerCharacter = await _playerCharacterService.GetPlayerCharacterByIdAsync(request.PlayerCharacterId);
            if (playerCharacter == null)
            {
                return new BuyItemResponseDTO { Success = false, Message = "Player Character not found." };
            }

            var item = await _itemService.GetItemByIdAsync(request.ItemId);
            // Đảm bảo Item và Price không null trước khi sử dụng
            if (item == null || item.Price == null) 
            {
                return new BuyItemResponseDTO { Success = false, Message = "Item not found or price is invalid." };
            }

            var totalPrice = item.Price.Value * request.Quantity;
            if (playerCharacter.Total_coin < totalPrice) 
            {
                return new BuyItemResponseDTO { Success = false, Message = "Insufficient Total_Coin." };
            }
            
            await _playerCharacterService.UpdatePlayerTotalMoneyAsync(playerCharacter.Player_Character_id, -totalPrice);
            var newTransaction = new Transactions
            {
                Player_CharacterId = request.PlayerCharacterId,
                Trandate = DateTime.Now,
                Price = totalPrice,
                TotalAmount = request.Quantity
            };
            _context.Transactions.Add(newTransaction);
            await _context.SaveChangesAsync();
            var newTransactionItem = new TransactionItem
            {
                Transaction_id = newTransaction.Transaction_id, // Sử dụng ID từ Transactions vừa tạo
                Item_id = request.ItemId,
                Quantity = request.Quantity,
                UnitPrice = (int)item.Price.Value // Ép kiểu vì UnitPrice là int
            };
            _context.TransactionItem.Add(newTransactionItem);
            await _context.SaveChangesAsync();


            await _playerCharacterService.AddOrUpdateInventoryItemAsync(request.PlayerCharacterId, request.ItemId, request.Quantity);

            await transaction.CommitAsync();

            // Lấy lại số tiền mới nhất của player và playerCharacter để trả về phản hồi
            var updatedPlayerCharacter = await _playerCharacterService.GetPlayerCharacterByIdAsync(request.PlayerCharacterId);

            return new BuyItemResponseDTO
            {
                Success = true,
                Message = $"Successfully purchased {request.Quantity} x {item.Name}.",
                NewPlayerTotalCoin = updatedPlayerCharacter?.Total_coin ?? 0
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return new BuyItemResponseDTO { Success = false, Message = $"Purchase failed: {ex.InnerException}" };
        }
    }
}