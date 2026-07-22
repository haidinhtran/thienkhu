using System.Text.Json;

namespace CultivationApi.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; } // PK
    public Guid CharacterId { get; set; } // FK
    public required string ActionType { get; set; } // EXP_GAIN, ITEM_CONSUME, BREAKTHROUGH
    public JsonDocument? Details { get; set; } // JSONB
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Character Character { get; set; } = null!;
}
