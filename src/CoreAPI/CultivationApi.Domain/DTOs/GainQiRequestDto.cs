namespace CultivationApi.Domain.DTOs;

public record GainQiRequestDto
{
    public required string DiscordId { get; init; }
    public required string ServerId { get; init; }
    public required string Username { get; init; }
}
