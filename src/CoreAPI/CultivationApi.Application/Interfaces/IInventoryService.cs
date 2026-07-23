using CultivationApi.Domain.Entities;

namespace CultivationApi.Application.Interfaces;

public interface IInventoryService
{
    Task<Inventory> GetOrCreateInventoryAsync(Guid characterId);
    Task<bool> AddItemAsync(Guid characterId, string itemId, int quantity, string itemType);
    Task<bool> RemoveItemAsync(Guid characterId, string itemId, int quantity);
    Task<bool> HasItemAsync(Guid characterId, string itemId, int quantity);
    Task<bool> EquipItemAsync(Guid characterId, string itemId, string slot);
}
