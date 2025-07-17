namespace DATNBACK_END.Services.Item;
using DATNBACK_END.Models;
public interface IItemService
{
    // Lấy tất cả các vật phẩm để hiển thị trong shop (trả về Item Model trực tiếp)
    Task<List<Item>> GetAllItemsAsync();
    // Lấy thông tin một vật phẩm cụ thể (trả về Item Model trực tiếp)
    Task<Item?> GetItemByIdAsync(int itemId);
    // Lấy Item tiền tệ theo tên và loại
    Task<Item?> GetCurrencyItemByNameAndTypeAsync(string currencyName, string type);
}