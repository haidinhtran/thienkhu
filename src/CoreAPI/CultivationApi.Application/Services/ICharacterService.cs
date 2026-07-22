using CultivationApi.Domain.DTOs;

namespace CultivationApi.Application.Services;

public interface ICharacterService
{
    Task<CharacterProfileDto> GetOrCreateProfileAsync(string discordId, string serverId, string username, CancellationToken ct = default);
    Task<GainQiResultDto> AddPassiveQiAsync(string discordId, string serverId, string username, CancellationToken ct = default);
    Task<AscendResultDto> AscendAsync(string discordId, string serverId, CancellationToken ct = default);
}
