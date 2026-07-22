namespace CultivationApi.Domain.DTOs;

public record GainQiResultDto
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public long CurrentQi { get; init; }
    public long GainedQi { get; init; }
}
