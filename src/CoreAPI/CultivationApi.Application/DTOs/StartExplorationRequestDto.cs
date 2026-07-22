namespace CultivationApi.Application.DTOs;

public record StartExplorationRequestDto
{
    public string DiscordId { get; init; } = string.Empty;
    public string ServerId { get; init; } = string.Empty;
    public string LocationId { get; init; } = string.Empty;
}
