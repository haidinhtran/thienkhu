namespace CultivationApi.Application.DTOs;

public record ExplorationResultDto
{
    public bool Success { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Narrative { get; init; } = string.Empty;
    public long QiReward { get; init; }
    public int SpiritStonesReward { get; init; }
}
