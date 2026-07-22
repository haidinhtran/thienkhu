namespace CultivationApi.Domain.Entities;

public class ServerConfig
{
    public required string ServerId { get; set; } // PK
    public Dictionary<int, string> RealmNames { get; set; } = new(); // JSONB
    public int DailyQiLimit { get; set; } = 1000;
    public bool IsActive { get; set; } = true;
    
    public ICollection<Character> Characters { get; set; } = new List<Character>();
}
