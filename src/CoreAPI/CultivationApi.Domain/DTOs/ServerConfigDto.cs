namespace CultivationApi.Domain.DTOs;

public record ServerConfigDto
{
    public required string ServerId { get; init; }
    public List<string> ChatToEarnChannels { get; init; } = new();
    public bool IsActive { get; init; }
}
