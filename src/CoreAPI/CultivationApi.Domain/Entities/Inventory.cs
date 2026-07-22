namespace CultivationApi.Domain.Entities;

public class Inventory
{
    public Guid Id { get; set; } // PK
    public Guid CharacterId { get; set; } // FK
    
    public EquippedGear EquippedGear { get; set; } = new(); // JSONB
    public List<InventoryItem> Items { get; set; } = new(); // JSONB

    [System.ComponentModel.DataAnnotations.Timestamp]
    public uint Version { get; set; } // Concurrency

    public Character Character { get; set; } = null!;
}

public class EquippedGear
{
    public string? Head { get; set; }
    public string? Chest { get; set; }
    public string? Weapon { get; set; }
    public string? Artifact { get; set; }
}

public class InventoryItem
{
    public required string ItemId { get; set; }
    public int Quantity { get; set; }
    public required string ItemType { get; set; }
}
