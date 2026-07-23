using CultivationApi.Application.Interfaces;
using CultivationApi.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CultivationApi.Application.Services;

public class GameDataContainer
{
    public List<LevelConfig> LevelConfigs { get; set; } = new();
    public List<SecretDomainConfig> SecretDomains { get; set; } = new();
}

public class JsonGameConfigProvider : IGameConfigProvider
{
    private readonly GameDataContainer _data;
    
    public JsonGameConfigProvider(ILogger<JsonGameConfigProvider> logger)
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "game_data.json");
        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            _data = JsonSerializer.Deserialize<GameDataContainer>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new GameDataContainer();
            logger.LogInformation("Loaded game_data.json successfully with {count} levels.", _data.LevelConfigs.Count);
        }
        else
        {
            logger.LogWarning($"game_data.json not found at {filePath}");
            _data = new GameDataContainer();
        }
    }

    public LevelConfig? GetLevelConfig(int level, string serverId)
    {
        return _data.LevelConfigs.FirstOrDefault(l => l.Level == level);
    }

    public SecretDomainConfig? GetSecretDomainConfig(string domainId, string serverId)
    {
        return _data.SecretDomains.FirstOrDefault(d => d.DomainId == domainId);
    }

    public List<SecretDomainConfig> GetAllSecretDomains(string serverId)
    {
        return _data.SecretDomains;
    }
}
