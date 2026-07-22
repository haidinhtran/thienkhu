using System.Collections.Generic;

namespace CultivationApi.Application.DTOs;

public record ExplorationEventDto
{
    public string EventId { get; init; } = string.Empty;
    public string EventType { get; init; } = string.Empty; // "STORY" or "COMBAT"
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public List<ExplorationChoiceDto> Choices { get; init; } = new();
}

public record ExplorationChoiceDto
{
    public string ChoiceId { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string Style { get; init; } = "PRIMARY"; // PRIMARY, SUCCESS, DANGER, SECONDARY
}
