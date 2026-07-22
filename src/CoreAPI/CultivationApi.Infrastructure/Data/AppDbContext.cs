using CultivationApi.Application.Interfaces;
using CultivationApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CultivationApi.Infrastructure.Data;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<DiscordUser> DiscordUsers => Set<DiscordUser>();
    public DbSet<ServerConfig> ServerConfigs => Set<ServerConfig>();
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<Inventory> Inventories => Set<Inventory>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DiscordUser>(entity =>
        {
            entity.HasKey(e => e.DiscordId);
            entity.Property(e => e.DiscordId).HasMaxLength(255);
        });

        modelBuilder.Entity<ServerConfig>(entity =>
        {
            entity.HasKey(e => e.ServerId);
            entity.Property(e => e.ServerId).HasMaxLength(255);
            entity.Property(e => e.RealmNames)
                  .HasColumnType("jsonb")
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                      v => JsonSerializer.Deserialize<Dictionary<int, string>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<int, string>()
                  )
                  .Metadata.SetValueComparer(new ValueComparer<Dictionary<int, string>>(
                      (c1, c2) => JsonSerializer.Serialize(c1, (JsonSerializerOptions?)null) == JsonSerializer.Serialize(c2, (JsonSerializerOptions?)null),
                      c => JsonSerializer.Serialize(c, (JsonSerializerOptions?)null).GetHashCode(),
                      c => JsonSerializer.Deserialize<Dictionary<int, string>>(JsonSerializer.Serialize(c, (JsonSerializerOptions?)null), (JsonSerializerOptions?)null)!
                  ));
            
            entity.Property(e => e.ChatToEarnChannels)
                  .HasColumnType("jsonb")
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                      v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
                  )
                  .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                      (c1, c2) => JsonSerializer.Serialize(c1, (JsonSerializerOptions?)null) == JsonSerializer.Serialize(c2, (JsonSerializerOptions?)null),
                      c => JsonSerializer.Serialize(c, (JsonSerializerOptions?)null).GetHashCode(),
                      c => JsonSerializer.Deserialize<List<string>>(JsonSerializer.Serialize(c, (JsonSerializerOptions?)null), (JsonSerializerOptions?)null)!
                  ));
        });

        modelBuilder.Entity<Character>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Version).IsRowVersion();
            
            entity.OwnsOne(e => e.BaseStats, stats =>
            {
                stats.ToJson();
            });

            entity.HasOne(e => e.DiscordUser)
                  .WithMany(u => u.Characters)
                  .HasForeignKey(e => e.DiscordId);

            entity.HasOne(e => e.ServerConfig)
                  .WithMany(s => s.Characters)
                  .HasForeignKey(e => e.ServerId);
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Version).IsRowVersion();
            
            entity.HasOne(e => e.Character)
                  .WithOne(c => c.Inventory)
                  .HasForeignKey<Inventory>(e => e.CharacterId);
            
            entity.OwnsOne(e => e.EquippedGear, gear =>
            {
                gear.ToJson();
            });

            entity.Property(e => e.Items)
                  .HasColumnType("jsonb")
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                      v => JsonSerializer.Deserialize<List<InventoryItem>>(v, (JsonSerializerOptions?)null) ?? new List<InventoryItem>()
                  )
                  .Metadata.SetValueComparer(new ValueComparer<List<InventoryItem>>(
                      (c1, c2) => JsonSerializer.Serialize(c1, (JsonSerializerOptions?)null) == JsonSerializer.Serialize(c2, (JsonSerializerOptions?)null),
                      c => JsonSerializer.Serialize(c, (JsonSerializerOptions?)null).GetHashCode(),
                      c => JsonSerializer.Deserialize<List<InventoryItem>>(JsonSerializer.Serialize(c, (JsonSerializerOptions?)null), (JsonSerializerOptions?)null)!
                  ));
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Details).HasColumnType("jsonb");

            entity.HasOne(e => e.Character)
                  .WithMany(c => c.AuditLogs)
                  .HasForeignKey(e => e.CharacterId);
        });
    }
}
