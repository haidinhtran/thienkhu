using CultivationApi.Application.Interfaces;
using CultivationApi.Domain.DTOs;
using CultivationApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CultivationApi.Application.Services;

public class CharacterService : ICharacterService
{
    private readonly IAppDbContext _dbContext;

    public CharacterService(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CharacterProfileDto> GetOrCreateProfileAsync(string discordId, string serverId, string username, CancellationToken ct = default)
    {
        // 1. Ensure DiscordUser exists
        var user = await _dbContext.DiscordUsers.FirstOrDefaultAsync(u => u.DiscordId == discordId, ct);
        if (user == null)
        {
            user = new DiscordUser { DiscordId = discordId, Username = username, CreatedAt = DateTime.UtcNow };
            _dbContext.DiscordUsers.Add(user);
        }
        else if (user.Username != username)
        {
            user.Username = username; // Update username if it changed
        }

        // 2. Ensure ServerConfig exists
        var server = await _dbContext.ServerConfigs.FirstOrDefaultAsync(s => s.ServerId == serverId, ct);
        if (server == null)
        {
            server = new ServerConfig { ServerId = serverId };
            _dbContext.ServerConfigs.Add(server);
        }

        // 3. Ensure Character exists
        var character = await _dbContext.Characters
            .FirstOrDefaultAsync(c => c.DiscordId == discordId && c.ServerId == serverId, ct);

        if (character == null)
        {
            character = new Character
            {
                Id = Guid.NewGuid(),
                DiscordId = discordId,
                ServerId = serverId,
                BaseStats = new BaseStats { Strength = 10, Agility = 10, Luck = 5, Health = 100, Mana = 50 }
            };
            _dbContext.Characters.Add(character);
            
            // Also create an empty inventory
            var inventory = new Inventory
            {
                Id = Guid.NewGuid(),
                CharacterId = character.Id
            };
            _dbContext.Inventories.Add(inventory);
        }

        // Save changes if anything was created
        if (_dbContext.DiscordUsers.Local.Any() || _dbContext.ServerConfigs.Local.Any() || _dbContext.Characters.Local.Any())
        {
            await _dbContext.SaveChangesAsync(ct);
        }

        // Determine Realm Title
        var realmName = server.RealmNames.TryGetValue(character.NumericLevel, out var customName) 
            ? customName 
            : "Qi Condensation"; // Default MVP realm

        return new CharacterProfileDto
        {
            DiscordId = character.DiscordId,
            ServerId = character.ServerId,
            Username = user.Username,
            Level = character.NumericLevel,
            RealmName = realmName,
            CurrentQi = character.CurrentQi,
            DailyQiLimit = server.DailyQiLimit,
            SpiritStones = character.SpiritStones,
            CurrentState = character.CurrentState
        };
    }
}
