namespace CultivationApi.Domain.DTOs;

public class CharacterProfileDto
{
    public required string DiscordId { get; set; }
    public required string ServerId { get; set; }
    public required string Username { get; set; }
    public int Level { get; set; }
    public string RealmName { get; set; } = "Mortal";
    public long CurrentQi { get; set; }
    public int DailyQiLimit { get; set; }
    public int SpiritStones { get; set; }
    public string CurrentState { get; set; } = "IDLE";
    public long TargetQi { get; set; }
    public string? RequiredBreakthroughItemId { get; set; }
    public int RequiredBreakthroughItemQuantity { get; set; }
}
