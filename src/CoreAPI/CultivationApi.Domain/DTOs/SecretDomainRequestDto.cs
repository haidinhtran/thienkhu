namespace CultivationApi.Domain.DTOs;

public class SecretDomainRequestDto
{
    public required string DiscordId { get; set; }
    public required string ServerId { get; set; }
    public required string DomainId { get; set; }
}
