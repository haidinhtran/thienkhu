using CultivationApi.Application.DTOs;

namespace CultivationApi.Application.Services;

public interface IActivitiesService
{
    Task<ExplorationEventDto> StartExplorationAsync(string discordId, string serverId, string locationId, CancellationToken ct = default);
    Task<ExplorationResultDto> SubmitExplorationChoiceAsync(ExplorationChoiceRequestDto request, CancellationToken ct = default);
    Task<CultivationApi.Domain.DTOs.SecretDomainResultDto> EnterSecretDomainAsync(CultivationApi.Domain.DTOs.SecretDomainRequestDto request, CancellationToken ct = default);
}
