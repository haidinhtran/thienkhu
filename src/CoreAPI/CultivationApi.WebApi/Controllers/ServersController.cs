using CultivationApi.Application.Interfaces;
using CultivationApi.Domain.DTOs;
using CultivationApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CultivationApi.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ServersController : ControllerBase
{
    private readonly IAppDbContext _dbContext;

    public ServersController(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("{serverId}/config")]
    public async Task<ActionResult<ServerConfigDto>> GetConfig(string serverId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(serverId)) return BadRequest();

        var config = await _dbContext.ServerConfigs.FirstOrDefaultAsync(s => s.ServerId == serverId, ct);
        if (config == null)
        {
            // Auto create if not exists for MVP
            config = new ServerConfig { ServerId = serverId };
            _dbContext.ServerConfigs.Add(config);
            await _dbContext.SaveChangesAsync(ct);
        }

        return Ok(new ServerConfigDto
        {
            ServerId = config.ServerId,
            ChatToEarnChannels = config.ChatToEarnChannels,
            MinQiPerMessage = config.MinQiPerMessage,
            MaxQiPerMessage = config.MaxQiPerMessage,
            InsightMultiplier = config.InsightMultiplier,
            IsActive = config.IsActive
        });
    }
}
