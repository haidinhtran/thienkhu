namespace CultivationApi.Application.DTOs;

public record ExplorationChoiceRequestDto
{
    public string DiscordId { get; init; } = string.Empty;
    public string ServerId { get; init; } = string.Empty;
    public string EventId { get; init; } = string.Empty;
    public string ChoiceId { get; init; } = string.Empty;
}
