using System.ComponentModel.DataAnnotations;

namespace CultivationApi.Domain.Entities;

public class Character
{
    public Guid Id { get; set; } // PK
    public required string DiscordId { get; set; } // FK
    public required string ServerId { get; set; } // FK
    public string CurrentState { get; set; } = "IDLE";
    public int NumericLevel { get; set; } = 1;
    public long CurrentQi { get; set; } = 0;
    public long DailyQiAccumulated { get; set; } = 0;
    public int SpiritStones { get; set; } = 0;
    public BaseStats BaseStats { get; set; } = new(); // JSONB
    public DateTime? LastMeditated { get; set; }
    
    [Timestamp]
    public uint Version { get; set; } // Concurrency

    public DiscordUser DiscordUser { get; set; } = null!;
    public ServerConfig ServerConfig { get; set; } = null!;
    public Inventory? Inventory { get; set; }
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}

public class BaseStats
{
    public int Strength { get; set; }
    public int Agility { get; set; }
    public int Luck { get; set; }
    public int Health { get; set; }
    public int Mana { get; set; }
}
