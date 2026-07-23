using CultivationApi.Application.Interfaces;
using CultivationApi.Domain.DTOs;
using CultivationApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using CultivationApi.Domain.Constants;
using CultivationApi.Domain.Exceptions;

namespace CultivationApi.Application.Services;

public class CharacterService : ICharacterService
{
    private readonly IAppDbContext _dbContext;
    private readonly IGameConfigProvider _configProvider;
    private readonly Random _random = new();

    public CharacterService(IAppDbContext dbContext, IGameConfigProvider configProvider)
    {
        _dbContext = dbContext;
        _configProvider = configProvider;
    }

    public async Task<CharacterProfileDto?> GetProfileAsync(string discordId, string serverId, string username, CancellationToken ct = default)
    {
        var character = await _dbContext.Characters
            .Include(c => c.ServerConfig)
            .FirstOrDefaultAsync(c => c.DiscordId == discordId && c.ServerId == serverId, ct);

        if (character == null)
        {
            return null;
        }

        var user = await _dbContext.DiscordUsers.FirstOrDefaultAsync(u => u.DiscordId == discordId, ct);
        if (user != null && user.Username != username)
        {
            user.Username = username;
            await _dbContext.SaveChangesAsync(ct);
        }

        var realmName = character.ServerConfig?.RealmNames.TryGetValue(character.NumericLevel, out var customName) == true 
            ? customName 
            : "Qi Condensation";

        var currentConfig = _configProvider.GetLevelConfig(character.NumericLevel, serverId);
        long targetQi = currentConfig?.RequiredQi ?? 0;

        return new CharacterProfileDto
        {
            DiscordId = character.DiscordId,
            ServerId = character.ServerId,
            Username = username,
            Level = character.NumericLevel,
            RealmName = realmName,
            CurrentQi = character.CurrentQi,
            DailyQiLimit = character.ServerConfig?.DailyQiLimit ?? 1000,
            SpiritStones = character.SpiritStones,
            CurrentState = character.CurrentState,
            TargetQi = targetQi,
            RequiredBreakthroughItemId = currentConfig?.RequiredBreakthroughItemId,
            RequiredBreakthroughItemQuantity = currentConfig?.RequiredBreakthroughItemQuantity ?? 0,
            BaseStats = character.BaseStats
        };
    }

    public async Task<CharacterProfileDto> CreateCharacterAsync(string discordId, string serverId, string username, CancellationToken ct = default)
    {
        var user = await _dbContext.DiscordUsers.FirstOrDefaultAsync(u => u.DiscordId == discordId, ct);
        if (user == null)
        {
            user = new DiscordUser
            {
                DiscordId = discordId,
                Username = username,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.DiscordUsers.Add(user);
        }

        var server = await _dbContext.ServerConfigs.FirstOrDefaultAsync(s => s.ServerId == serverId, ct);
        if (server == null)
        {
            server = new ServerConfig { ServerId = serverId };
            _dbContext.ServerConfigs.Add(server);
        }

        var character = await _dbContext.Characters.FirstOrDefaultAsync(c => c.DiscordId == discordId && c.ServerId == serverId, ct);
        if (character != null)
        {
            throw new DomainException("Character already exists.");
        }

        var level1Config = _configProvider.GetLevelConfig(1, serverId);
        var baseStats = level1Config != null
            ? new BaseStats 
              { 
                  Strength = level1Config.BaseStats.Strength, 
                  Agility = level1Config.BaseStats.Agility, 
                  Luck = level1Config.BaseStats.Luck, 
                  Health = level1Config.BaseStats.Health, 
                  Mana = level1Config.BaseStats.Mana, 
                  Insight = level1Config.BaseStats.Insight 
              }
            : new BaseStats { Strength = 15, Agility = 15, Luck = 7, Health = 150, Mana = 70, Insight = 10 };

        character = new Character
        {
            Id = Guid.NewGuid(),
            DiscordId = discordId,
            ServerId = serverId,
            BaseStats = baseStats
        };
        _dbContext.Characters.Add(character);
        
        var inventory = new Inventory
        {
            Id = Guid.NewGuid(),
            CharacterId = character.Id
        };
        _dbContext.Inventories.Add(inventory);

        await _dbContext.SaveChangesAsync(ct);

        return (await GetProfileAsync(discordId, serverId, username, ct))!;
    }

    public async Task<GainQiResultDto> AddPassiveQiAsync(string discordId, string serverId, string username, CancellationToken ct = default)
    {
        var character = await _dbContext.Characters
            .Include(c => c.ServerConfig)
            .FirstOrDefaultAsync(c => c.DiscordId == discordId && c.ServerId == serverId, ct);
            
        if (character == null)
        {
            return new GainQiResultDto { Success = false, Message = "Please use /cultivate to create a character first." };
        }

        var serverConfig = character.ServerConfig;
        if (serverConfig == null || !serverConfig.IsActive)
        {
            return new GainQiResultDto { Success = false, Message = "Server is inactive." };
        }
        
        if (character.LastMeditated.HasValue && character.LastMeditated.Value.Date < DateTime.UtcNow.Date)
        {
            character.DailyQiAccumulated = 0;
        }

        if (character.LastMeditated.HasValue && 
            (DateTime.UtcNow - character.LastMeditated.Value).TotalSeconds < serverConfig.MessageCooldownSeconds)
        {
            return new GainQiResultDto 
            { 
                Success = false, 
                Message = "Cooldown active.",
                CurrentQi = character.CurrentQi
            };
        }

        if (character.DailyQiAccumulated >= serverConfig.DailyQiLimit)
        {
            return new GainQiResultDto 
            { 
                Success = false, 
                Message = "Daily Qi limit reached.",
                CurrentQi = character.CurrentQi
            };
        }

        int insight = character.BaseStats.Insight > 0 ? character.BaseStats.Insight : 10;
        int minQi = serverConfig.MinQiPerMessage;
        int upperLimit = minQi + (int)(insight * serverConfig.InsightMultiplier);
        int maxAllowed = Math.Min(upperLimit, serverConfig.MaxQiPerMessage);
        if (maxAllowed < minQi)
        {
            maxAllowed = minQi;
        }

        long qiToAdd = _random.Next(minQi, maxAllowed + 1);
        if (character.DailyQiAccumulated + qiToAdd > serverConfig.DailyQiLimit)
        {
            qiToAdd = serverConfig.DailyQiLimit - character.DailyQiAccumulated;
        }

        character.CurrentQi += qiToAdd;
        character.DailyQiAccumulated += qiToAdd;
        character.LastMeditated = DateTime.UtcNow;

        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            CharacterId = character.Id,
            ActionType = AuditLogTypes.ExpGain,
            Details = JsonDocument.Parse(JsonSerializer.Serialize(new { Source = "ChatToEarn", Amount = qiToAdd })),
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.AuditLogs.Add(auditLog);

        try
        {
            await _dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new DomainException("Concurrency error. Action conflicts with another request. Please try again.");
        }

        return new GainQiResultDto 
        { 
            Success = true, 
            Message = $"Gained {qiToAdd} Qi.",
            CurrentQi = character.CurrentQi,
            GainedQi = qiToAdd
        };
    }

    public async Task<AscendResultDto> AscendAsync(string discordId, string serverId, CancellationToken ct = default)
    {
        var character = await _dbContext.Characters
            .Include(c => c.Inventory)
            .FirstOrDefaultAsync(c => c.DiscordId == discordId && c.ServerId == serverId, ct);

        if (character == null)
        {
            return new AscendResultDto { Success = false, Message = "Character not found." };
        }

        var currentConfig = _configProvider.GetLevelConfig(character.NumericLevel, serverId);
        if (currentConfig == null)
        {
            return new AscendResultDto { Success = false, Message = "Already at max level." };
        }

        if (character.CurrentQi < currentConfig.RequiredQi)
        {
            return new AscendResultDto { Success = false, Message = "Not enough Qi to ascend." };
        }

        // Check required items
        if (!string.IsNullOrEmpty(currentConfig.RequiredBreakthroughItemId) && currentConfig.RequiredBreakthroughItemQuantity > 0)
        {
            if (character.Inventory == null)
            {
                return new AscendResultDto { Success = false, Message = "Missing required breakthrough items." };
            }

            var item = character.Inventory.Items.FirstOrDefault(i => i.ItemId == currentConfig.RequiredBreakthroughItemId);
            if (item == null || item.Quantity < currentConfig.RequiredBreakthroughItemQuantity)
            {
                return new AscendResultDto { Success = false, Message = "Missing required breakthrough items." };
            }

            // Consume item
            item.Quantity -= currentConfig.RequiredBreakthroughItemQuantity;
            if (item.Quantity == 0)
            {
                character.Inventory.Items.Remove(item);
            }
            // Force EF to detect change
            character.Inventory.Items = new List<InventoryItem>(character.Inventory.Items);
        }

        // Apply ascension
        int oldLevel = character.NumericLevel;
        character.NumericLevel += 1;
        
        // Reset or deduct Qi? MVP: Reset Qi. Or deduct? Let's deduct to save overflow.
        character.CurrentQi -= currentConfig.RequiredQi;

        // Apply new stats
        var nextConfig = _configProvider.GetLevelConfig(character.NumericLevel, serverId);
        if (nextConfig != null)
        {
            character.BaseStats.Strength = nextConfig.BaseStats.Strength;
            character.BaseStats.Agility = nextConfig.BaseStats.Agility;
            character.BaseStats.Luck = nextConfig.BaseStats.Luck;
            character.BaseStats.Health = nextConfig.BaseStats.Health;
            character.BaseStats.Mana = nextConfig.BaseStats.Mana;
            character.BaseStats.Insight = nextConfig.BaseStats.Insight;
        }

        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            CharacterId = character.Id,
            ActionType = AuditLogTypes.Breakthrough,
            Details = JsonDocument.Parse(JsonSerializer.Serialize(new { OldLevel = oldLevel, NewLevel = character.NumericLevel })),
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.AuditLogs.Add(auditLog);

        try
        {
            await _dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new DomainException("Concurrency error. Please try ascending again.");
        }

        return new AscendResultDto
        {
            Success = true,
            Message = "Ascension successful!",
            OldLevel = oldLevel,
            NewLevel = character.NumericLevel,
            NewBaseStats = character.BaseStats
        };
    }
}
