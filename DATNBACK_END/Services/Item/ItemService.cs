namespace DATNBACK_END.Services.Item; 
using DATNBACK_END.Models;
using Microsoft.EntityFrameworkCore;

public class ItemService : IItemService
{
    private readonly AppDataContexts.AppDataContext _context;

    public ItemService(AppDataContexts.AppDataContext context)
    {
        _context = context;
    }
    public async Task<List<Item>> GetAllItemsAsync()
    {
        return await _context.items.ToListAsync();
    }
    public async Task<Item?> GetItemByIdAsync(int itemId)
    {
        return await _context.items.FirstOrDefaultAsync(i => i.Item_id == itemId);
    }

    public async Task<Item?> GetCurrencyItemByNameAndTypeAsync(string currencyName, string type)
    {
        return await _context.items.FirstOrDefaultAsync(i => i.Type == type && i.Name == currencyName);
    }
}