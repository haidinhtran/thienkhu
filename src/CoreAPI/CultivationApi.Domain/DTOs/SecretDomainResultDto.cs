namespace CultivationApi.Domain.DTOs;

public class SecretDomainResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsVictory { get; set; }
    public List<string> BattleLog { get; set; } = new();
    public int RewardSpiritStones { get; set; }
    public List<RewardItemDto> RewardItems { get; set; } = new();
}

public class RewardItemDto
{
    public required string ItemId { get; set; }
    public int Quantity { get; set; }
    public required string ItemType { get; set; }
}
