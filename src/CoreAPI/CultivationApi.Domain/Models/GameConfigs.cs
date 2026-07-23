namespace CultivationApi.Domain.Models;

public class LevelConfig
{
    public int Level { get; set; }
    public long RequiredQi { get; set; }
    public string? RequiredBreakthroughItemId { get; set; }
    public int RequiredBreakthroughItemQuantity { get; set; }
    public BaseStatsConfig BaseStats { get; set; } = new();
}

public class BaseStatsConfig
{
    public int Strength { get; set; }
    public int Agility { get; set; }
    public int Luck { get; set; }
    public int Health { get; set; }
    public int Mana { get; set; }
    public int Insight { get; set; } = 10;
}

public class SecretDomainConfig
{
    public required string DomainId { get; set; }
    public required string Name { get; set; }
    public int RequiredLevel { get; set; }
    public BaseStatsConfig BossStats { get; set; } = new();
    
    // Simple reward pool for MVP
    public int RewardSpiritStones { get; set; }
    public List<DomainRewardItem> RewardItems { get; set; } = new();
}

public class DomainRewardItem
{
    public required string ItemId { get; set; }
    public int Quantity { get; set; }
    public required string ItemType { get; set; } // e.g. "Consumable", "Material"
    public double DropRate { get; set; } // 0.0 to 1.0
}
