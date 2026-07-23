namespace CultivationApi.Domain.DTOs;

public record ServerConfigDto
{
    public required string ServerId { get; init; }
    public List<string> ChatToEarnChannels { get; init; } = new();
    public int MinQiPerMessage { get; init; } = 10;
    public int MaxQiPerMessage { get; init; } = 100;
    public double InsightMultiplier { get; init; } = 1.0;
    public bool IsActive { get; init; }
}
