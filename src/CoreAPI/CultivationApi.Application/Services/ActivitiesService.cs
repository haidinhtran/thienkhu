using CultivationApi.Application.DTOs;
using CultivationApi.Application.Interfaces;
using CultivationApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using CultivationApi.Domain.Constants;
using CultivationApi.Domain.Exceptions;

namespace CultivationApi.Application.Services;

public class ActivitiesService : IActivitiesService
{
    private readonly IAppDbContext _dbContext;
    private readonly IGameConfigProvider _configProvider;
    private readonly Random _random;

    public ActivitiesService(IAppDbContext dbContext, IGameConfigProvider configProvider)
    {
        _dbContext = dbContext;
        _configProvider = configProvider;
        _random = new Random();
    }

    public async Task<ExplorationEventDto> StartExplorationAsync(string discordId, string serverId, string locationId, CancellationToken ct = default)
    {
        var character = await _dbContext.Characters
            .FirstOrDefaultAsync(c => c.DiscordId == discordId && c.ServerId == serverId, ct);

        if (character == null)
            throw new DomainException("Character not found.");

        if (character.CurrentState != CharacterStates.Idle)
            throw new DomainException($"Character is currently {character.CurrentState}. You must be IDLE to explore.");

        character.CurrentState = CharacterStates.InExploration;
        
        try
        {
            await _dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new DomainException("Action conflicts with another request. Please try again.");
        }

        // Simple RNG for MVP
        bool isCombat = _random.NextDouble() > 0.5;

        if (isCombat)
        {
            return new ExplorationEventDto
            {
                EventId = Guid.NewGuid().ToString(),
                EventType = "COMBAT",
                Title = "Wild Beast Appears!",
                Description = "A ferocious spirit beast blocks your path.",
                Choices = new List<ExplorationChoiceDto>
                {
                    new ExplorationChoiceDto { ChoiceId = "attack", Label = "Attack", Style = "DANGER" },
                    new ExplorationChoiceDto { ChoiceId = "flee", Label = "Flee", Style = "SECONDARY" }
                }
            };
        }
        else
        {
            return new ExplorationEventDto
            {
                EventId = Guid.NewGuid().ToString(),
                EventType = "STORY",
                Title = "Mysterious Cave",
                Description = "You discover a hidden cave radiating spiritual energy.",
                Choices = new List<ExplorationChoiceDto>
                {
                    new ExplorationChoiceDto { ChoiceId = "enter", Label = "Enter Cave", Style = "PRIMARY" },
                    new ExplorationChoiceDto { ChoiceId = "ignore", Label = "Walk Away", Style = "SECONDARY" }
                }
            };
        }
    }

    public async Task<ExplorationResultDto> SubmitExplorationChoiceAsync(ExplorationChoiceRequestDto request, CancellationToken ct = default)
    {
        var character = await _dbContext.Characters
            .FirstOrDefaultAsync(c => c.DiscordId == request.DiscordId && c.ServerId == request.ServerId, ct);

        if (character == null)
            throw new DomainException("Character not found.");

        if (character.CurrentState != CharacterStates.InExploration)
            throw new DomainException("Character is not exploring.");

        character.CurrentState = CharacterStates.Idle;
        
        bool success = false;
        long qiReward = 0;
        int stoneReward = 0;
        string narrative = "";

        if (request.ChoiceId == "attack")
        {
            int roll = _random.Next(1, 100);
            success = roll > 30; // 70% win rate for MVP
            
            if (success)
            {
                qiReward = 50 * character.NumericLevel;
                stoneReward = 10;
                narrative = "You defeated the beast and harvested its core!";
            }
            else
            {
                narrative = "You were overpowered and had to retreat.";
            }
        }
        else if (request.ChoiceId == "flee")
        {
            success = true;
            narrative = "You safely escaped.";
        }
        else if (request.ChoiceId == "enter")
        {
            int roll = _random.Next(1, 100);
            success = roll > 20; // 80% chance for good event
            if (success)
            {
                qiReward = 100 * character.NumericLevel;
                stoneReward = 20;
                narrative = "You found a stash of ancient spirit stones and absorbed rich ambient Qi!";
            }
            else
            {
                narrative = "It was a trap! You barely escaped with your life.";
            }
        }
        else
        {
            success = true;
            narrative = "You decided not to take the risk and continued your journey.";
        }

        if (success && (qiReward > 0 || stoneReward > 0))
        {
            character.CurrentQi += qiReward;
            character.SpiritStones += stoneReward;

            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                CharacterId = character.Id,
                ActionType = AuditLogTypes.ExplorationReward,
                Details = JsonDocument.Parse(JsonSerializer.Serialize(new { Qi = qiReward, SpiritStones = stoneReward })),
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.AuditLogs.Add(auditLog);
        }

        try
        {
            await _dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new DomainException("Action conflicts with another request. Please try again.");
        }

        return new ExplorationResultDto
        {
            Success = success,
            Title = "Exploration Completed",
            Narrative = narrative,
            QiReward = qiReward,
            SpiritStonesReward = stoneReward
        };
    }

    public async Task<CultivationApi.Domain.DTOs.SecretDomainResultDto> EnterSecretDomainAsync(CultivationApi.Domain.DTOs.SecretDomainRequestDto request, CancellationToken ct = default)
    {
        var character = await _dbContext.Characters
            .Include(c => c.Inventory)
            .FirstOrDefaultAsync(c => c.DiscordId == request.DiscordId && c.ServerId == request.ServerId, ct);

        if (character == null)
            throw new DomainException("Character not found.");

        if (character.CurrentState != CharacterStates.Idle)
            throw new DomainException($"Character is currently {character.CurrentState}. You must be IDLE to enter a Secret Domain.");

        var domainConfig = _configProvider.GetSecretDomainConfig(request.DomainId, request.ServerId);
        if (domainConfig == null)
            throw new DomainException("Secret Domain not found.");

        if (character.NumericLevel < domainConfig.RequiredLevel)
            throw new DomainException($"You must be at least level {domainConfig.RequiredLevel} to enter this domain.");

        // Simulate combat (MVP: compare stats and add some RNG)
        int playerPower = character.BaseStats.Strength + character.BaseStats.Agility + (character.BaseStats.Health / 10);
        
        if (character.Inventory?.EquippedGear != null)
        {
            if (!string.IsNullOrEmpty(character.Inventory.EquippedGear.Weapon)) playerPower += 10;
            if (!string.IsNullOrEmpty(character.Inventory.EquippedGear.Chest)) playerPower += 10;
            if (!string.IsNullOrEmpty(character.Inventory.EquippedGear.Artifact)) playerPower += 5;
        }

        int bossPower = domainConfig.BossStats.Strength + domainConfig.BossStats.Agility + (domainConfig.BossStats.Health / 10);
        
        // Add Luck factor
        playerPower += _random.Next(0, character.BaseStats.Luck);
        bossPower += _random.Next(0, 10); // Boss has fixed random variance

        bool isVictory = playerPower >= bossPower;
        
        var battleLog = new List<string>
        {
            $"You entered {domainConfig.Name}.",
            $"You encountered the domain boss! (Boss Power: {bossPower}, Your Power: {playerPower})"
        };

        var resultDto = new CultivationApi.Domain.DTOs.SecretDomainResultDto
        {
            Success = true,
            IsVictory = isVictory,
            BattleLog = battleLog
        };

        if (isVictory)
        {
            battleLog.Add("You emerged victorious!");
            
            // Give Rewards
            character.SpiritStones += domainConfig.RewardSpiritStones;
            resultDto.RewardSpiritStones = domainConfig.RewardSpiritStones;

            if (character.Inventory == null)
            {
                character.Inventory = new Inventory { CharacterId = character.Id };
                _dbContext.Inventories.Add(character.Inventory);
            }

            foreach (var reward in domainConfig.RewardItems)
            {
                if (_random.NextDouble() <= reward.DropRate)
                {
                    var existingItem = character.Inventory.Items.FirstOrDefault(i => i.ItemId == reward.ItemId);
                    if (existingItem != null)
                    {
                        existingItem.Quantity += reward.Quantity;
                    }
                    else
                    {
                        character.Inventory.Items.Add(new InventoryItem
                        {
                            ItemId = reward.ItemId,
                            Quantity = reward.Quantity,
                            ItemType = reward.ItemType
                        });
                    }

                    resultDto.RewardItems.Add(new CultivationApi.Domain.DTOs.RewardItemDto
                    {
                        ItemId = reward.ItemId,
                        Quantity = reward.Quantity,
                        ItemType = reward.ItemType
                    });
                    
                    battleLog.Add($"Looted: {reward.Quantity}x {reward.ItemId}");
                }
            }

            // Force EF core to track JSONB list modification
            character.Inventory.Items = new List<InventoryItem>(character.Inventory.Items);

            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                CharacterId = character.Id,
                ActionType = "SECRET_DOMAIN_VICTORY",
                Details = JsonDocument.Parse(JsonSerializer.Serialize(new { DomainId = domainConfig.DomainId, Rewards = resultDto.RewardItems })),
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.AuditLogs.Add(auditLog);
        }
        else
        {
            battleLog.Add("You were defeated and forced to retreat...");
            // Optionally add penalty here (e.g. lose Qi or items)
        }

        try
        {
            await _dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new DomainException("Concurrency error. Please try again.");
        }

        return resultDto;
    }
}
