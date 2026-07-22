using CultivationApi.Domain.Models;

namespace CultivationApi.Application.Interfaces;

public interface IGameConfigProvider
{
    LevelConfig? GetLevelConfig(int level);
    SecretDomainConfig? GetSecretDomainConfig(string domainId);
    List<SecretDomainConfig> GetAllSecretDomains();
}
