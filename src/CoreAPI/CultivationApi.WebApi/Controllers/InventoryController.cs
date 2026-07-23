using CultivationApi.Application.Interfaces;
using CultivationApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CultivationApi.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;
    private readonly IAppDbContext _dbContext;

    public InventoryController(IInventoryService inventoryService, IAppDbContext dbContext)
    {
        _inventoryService = inventoryService;
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<Inventory>> GetInventory(
        [FromQuery] string discordId,
        [FromQuery] string serverId,
        CancellationToken ct)
    {
        var character = await _dbContext.Characters
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.DiscordId == discordId && c.ServerId == serverId, ct);

        if (character == null)
            return NotFound(new ProblemDetails { Title = "Not Found", Detail = "Character not found." });

        var inventory = await _inventoryService.GetOrCreateInventoryAsync(character.Id);
        return Ok(inventory);
    }

    [HttpPost("equip")]
    public async Task<IActionResult> EquipItem(
        [FromQuery] string discordId,
        [FromQuery] string serverId,
        [FromQuery] string itemId,
        [FromQuery] string slot,
        CancellationToken ct)
    {
        var character = await _dbContext.Characters
            .FirstOrDefaultAsync(c => c.DiscordId == discordId && c.ServerId == serverId, ct);

        if (character == null)
            return NotFound(new ProblemDetails { Title = "Not Found", Detail = "Character not found." });

        var success = await _inventoryService.EquipItemAsync(character.Id, itemId, slot);
        if (!success)
            return BadRequest(new ProblemDetails { Title = "Equip Failed", Detail = "Could not equip item. Ensure it exists in inventory and is equipment." });

        return Ok(new { Success = true });
    }
}
