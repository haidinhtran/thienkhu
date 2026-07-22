using CultivationApi.Application.Interfaces;
using CultivationApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CultivationApi.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IAppDbContext _context;

    public InventoryService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Inventory> GetOrCreateInventoryAsync(Guid characterId)
    {
        var inventory = await _context.Inventories
            .FirstOrDefaultAsync(i => i.CharacterId == characterId);

        if (inventory == null)
        {
            inventory = new Inventory
            {
                CharacterId = characterId,
                Items = new List<InventoryItem>(),
                EquippedGear = new EquippedGear()
            };
            _context.Inventories.Add(inventory);
            await _context.SaveChangesAsync(default);
        }

        return inventory;
    }

    public async Task<bool> AddItemAsync(Guid characterId, string itemId, int quantity, string itemType)
    {
        if (quantity <= 0) return false;

        var inventory = await GetOrCreateInventoryAsync(characterId);
        
        var existingItem = inventory.Items.FirstOrDefault(i => i.ItemId == itemId);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            inventory.Items.Add(new InventoryItem
            {
                ItemId = itemId,
                Quantity = quantity,
                ItemType = itemType
            });
        }

        // Force EF Core to detect JSONB changes by creating a new list reference
        inventory.Items = new List<InventoryItem>(inventory.Items);
        
        await _context.SaveChangesAsync(default);
        return true;
    }

    public async Task<bool> RemoveItemAsync(Guid characterId, string itemId, int quantity)
    {
        if (quantity <= 0) return false;

        var inventory = await GetOrCreateInventoryAsync(characterId);
        
        var existingItem = inventory.Items.FirstOrDefault(i => i.ItemId == itemId);
        if (existingItem == null || existingItem.Quantity < quantity)
        {
            return false; // Not enough items
        }

        existingItem.Quantity -= quantity;
        if (existingItem.Quantity == 0)
        {
            inventory.Items.Remove(existingItem);
        }

        // Force EF Core to detect JSONB changes
        inventory.Items = new List<InventoryItem>(inventory.Items);
        
        await _context.SaveChangesAsync(default);
        return true;
    }

    public async Task<bool> HasItemAsync(Guid characterId, string itemId, int quantity)
    {
        var inventory = await _context.Inventories
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.CharacterId == characterId);

        if (inventory == null) return false;

        var existingItem = inventory.Items.FirstOrDefault(i => i.ItemId == itemId);
        return existingItem != null && existingItem.Quantity >= quantity;
    }
}
