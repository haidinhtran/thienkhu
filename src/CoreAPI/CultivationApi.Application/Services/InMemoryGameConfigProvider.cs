using CultivationApi.Application.Interfaces;
using CultivationApi.Domain.Models;

namespace CultivationApi.Application.Services;

public class InMemoryGameConfigProvider : IGameConfigProvider
{
    private readonly Dictionary<int, LevelConfig> _levelConfigs = new();
    private readonly Dictionary<string, SecretDomainConfig> _secretDomains = new();

    public InMemoryGameConfigProvider()
    {
        InitializeLevelConfigs();
        InitializeSecretDomains();
    }

    private void InitializeLevelConfigs()
    {
        // MVP: Hardcoded 10 levels
        long baseQi = 100;
        for (int i = 1; i <= 10; i++)
        {
            _levelConfigs[i] = new LevelConfig
            {
                Level = i,
                RequiredQi = baseQi * i * i, // Exponential Qi scaling
                RequiredBreakthroughItemId = (i % 3 == 0) ? "breakthrough_pill_1" : null, // Every 3 levels require a pill
                RequiredBreakthroughItemQuantity = (i % 3 == 0) ? 1 : 0,
                BaseStats = new BaseStatsConfig
                {
                    Strength = 10 + (i * 5),
                    Agility = 10 + (i * 5),
                    Luck = 5 + (i * 2),
                    Health = 100 + (i * 50),
                    Mana = 50 + (i * 20)
                }
            };
        }
    }

    private void InitializeSecretDomains()
    {
        _secretDomains["goblin_cave"] = new SecretDomainConfig
        {
            DomainId = "goblin_cave",
            Name = "Goblin Cave",
            RequiredLevel = 1,
            RewardSpiritStones = 50,
            BossStats = new BaseStatsConfig { Health = 120, Strength = 12, Agility = 10 },
            RewardItems = new List<DomainRewardItem>
            {
                new DomainRewardItem { ItemId = "breakthrough_pill_1", Quantity = 1, ItemType = "Consumable", DropRate = 0.5 } // 50% drop rate
            }
        };

        _secretDomains["azure_mountain"] = new SecretDomainConfig
        {
            DomainId = "azure_mountain",
            Name = "Azure Cloud Mountain",
            RequiredLevel = 4,
            RewardSpiritStones = 200,
            BossStats = new BaseStatsConfig { Health = 400, Strength = 35, Agility = 25 },
            RewardItems = new List<DomainRewardItem>
            {
                new DomainRewardItem { ItemId = "breakthrough_pill_1", Quantity = 2, ItemType = "Consumable", DropRate = 0.8 },
                new DomainRewardItem { ItemId = "spirit_herb", Quantity = 5, ItemType = "Material", DropRate = 1.0 }
            }
        };
    }

    public LevelConfig? GetLevelConfig(int level)
    {
        return _levelConfigs.TryGetValue(level, out var config) ? config : null;
    }

    public SecretDomainConfig? GetSecretDomainConfig(string domainId)
    {
        return _secretDomains.TryGetValue(domainId, out var config) ? config : null;
    }

    public List<SecretDomainConfig> GetAllSecretDomains()
    {
        return _secretDomains.Values.ToList();
    }
}
