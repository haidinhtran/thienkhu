using CultivationApi.Domain.Models;

namespace CultivationApi.Application.Interfaces;

public interface IGameConfigProvider
{
    LevelConfig? GetLevelConfig(int level, string serverId);
    SecretDomainConfig? GetSecretDomainConfig(string domainId, string serverId);
    List<SecretDomainConfig> GetAllSecretDomains(string serverId);
}
