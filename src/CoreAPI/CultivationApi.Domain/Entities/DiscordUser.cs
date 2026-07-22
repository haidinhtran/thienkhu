namespace CultivationApi.Domain.Entities;

public class DiscordUser
{
    public required string DiscordId { get; set; } // PK
    public required string Username { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public ICollection<Character> Characters { get; set; } = new List<Character>();
}
