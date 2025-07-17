namespace DATNBACK_END.Models.DTO.Transaction;

// BuyItemRequestDTO: Yêu cầu mua vật phẩm từ client
public class BuyItemRequestDTO
{
    public int PlayerCharacterId { get; set; } // ID của nhân vật thực hiện mua
    public int ItemId { get; set; }
    public int Quantity { get; set; }
}

// BuyItemResponseDTO: Phản hồi sau khi mua vật phẩm
public class BuyItemResponseDTO
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public decimal NewPlayerTotalCoin { get; set; } // Total_Coin mới của Player_Character
}

// ExchangeCurrencyRequestDTO: Yêu cầu đổi tiền từ client
public class ExchangeCurrencyRequestDTO
{
    public int PlayerId { get; set; } // ID của Player (vì Total_Money thuộc về Player)
    public int PlayerCharacterId { get; set; } // ID của Player_Character (vì Total_coin thuộc về Player_Character)
    public string? SourceCurrencyItemName { get; set; } // Tên vật phẩm tiền tệ nguồn (ví dụ: "Money", "Coin")
    public string? TargetCurrencyItemName { get; set; } // Tên vật phẩm tiền tệ đích (ví dụ: "Money", "Coin")
    public decimal AmountToExchange { get; set; } // Số lượng tiền muốn đổi
}

// ExchangeCurrencyResponseDTO: Phản hồi sau khi đổi tiền
public class ExchangeCurrencyResponseDTO
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public decimal NewPlayerTotalMoney { get; set; } // Số Total_Money mới của Player
    public decimal NewPlayerTotalCoin { get; set; } // Số Total_Coin mới của Player_Character
}

// ConsumeItemRequestDTO: Yêu cầu tiêu thụ vật phẩm từ client
public class ConsumeItemRequestDTO
{
    public int PlayerCharacterId { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; } // Số lượng vật phẩm muốn tiêu thụ
}

// PlayerInventoryItemDTO: DTO biểu diễn một vật phẩm trong kho đồ của người chơi
public class PlayerInventoryItemDTO
{
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? ItemDescription { get; set; }
    public string? IconPath { get; set; } // Nếu có cột IconPath trong bảng Item
}