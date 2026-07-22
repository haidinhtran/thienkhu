using CultivationApi.Domain.Entities;

namespace CultivationApi.Domain.DTOs;

public class AscendResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int OldLevel { get; set; }
    public int NewLevel { get; set; }
    public BaseStats? NewBaseStats { get; set; }
}
