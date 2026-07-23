namespace CultivationApi.Domain.Entities;

public class ServerConfig
{
    public required string ServerId { get; set; } // PK
    public Dictionary<int, string> RealmNames { get; set; } = new(); // JSONB
    public int DailyQiLimit { get; set; } = 1000;
    public List<string> ChatToEarnChannels { get; set; } = new(); // JSONB
    public int MinQiPerMessage { get; set; } = 10;
    public int MaxQiPerMessage { get; set; } = 100;
    public double InsightMultiplier { get; set; } = 1.0;
    public int MessageCooldownSeconds { get; set; } = 60;
    public bool IsActive { get; set; } = true;
    
    public ICollection<Character> Characters { get; set; } = new List<Character>();
}
