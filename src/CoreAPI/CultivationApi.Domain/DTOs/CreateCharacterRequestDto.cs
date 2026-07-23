namespace CultivationApi.Domain.DTOs;

public record CreateCharacterRequestDto
{
    public required string DiscordId { get; init; }
    public required string ServerId { get; init; }
    public required string Username { get; init; }
}
