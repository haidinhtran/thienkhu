namespace CultivationApi.Domain.Entities;

public class ServerConfig
{
    public required string ServerId { get; set; } // PK
    public Dictionary<int, string> RealmNames { get; set; } = new(); // JSONB
    public int DailyQiLimit { get; set; } = 1000;
    public List<string> ChatToEarnChannels { get; set; } = new(); // JSONB
    public int QiPerMessage { get; set; } = 10;
    public int MessageCooldownSeconds { get; set; } = 60;
    public bool IsActive { get; set; } = true;
    
    public ICollection<Character> Characters { get; set; } = new List<Character>();
}
