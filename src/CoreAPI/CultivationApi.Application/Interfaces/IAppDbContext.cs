using CultivationApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CultivationApi.Application.Interfaces;

public interface IAppDbContext
{
    DbSet<DiscordUser> DiscordUsers { get; }
    DbSet<ServerConfig> ServerConfigs { get; }
    DbSet<Character> Characters { get; }
    DbSet<Inventory> Inventories { get; }
    DbSet<AuditLog> AuditLogs { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
