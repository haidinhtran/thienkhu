using CultivationApi.Application.Services;
using CultivationApi.Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CultivationApi.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CharactersController : ControllerBase
{
    private readonly ICharacterService _characterService;

    public CharactersController(ICharacterService characterService)
    {
        _characterService = characterService;
    }

    [HttpGet("profile")]
    public async Task<ActionResult<CharacterProfileDto>> GetProfile(
        [FromQuery] string discordId, 
        [FromQuery] string serverId,
        [FromQuery] string username,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(discordId) || string.IsNullOrWhiteSpace(serverId) || string.IsNullOrWhiteSpace(username))
        {
            return BadRequest(new ProblemDetails 
            { 
                Title = "Validation Error", 
                Detail = "discordId, serverId, and username are required." 
            });
        }

        var profile = await _characterService.GetOrCreateProfileAsync(discordId, serverId, username, ct);
        return Ok(profile);
    }

    [HttpPost("gain-qi")]
    public async Task<ActionResult<GainQiResultDto>> GainQi(
        [FromBody] GainQiRequestDto request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.DiscordId) || string.IsNullOrWhiteSpace(request.ServerId) || string.IsNullOrWhiteSpace(request.Username))
        {
            return BadRequest(new ProblemDetails 
            { 
                Title = "Validation Error", 
                Detail = "discordId, serverId, and username are required." 
            });
        }

        var result = await _characterService.AddPassiveQiAsync(request.DiscordId, request.ServerId, request.Username, ct);
        
        if (!result.Success)
        {
            // Even if failed due to cooldown, it's a valid request but failed business logic.
            // Returning 200 with Success=false is fine for this game loop, or 400 Bad Request.
            // Let's return 400 for business validation failure.
            return BadRequest(result);
        }

        return Ok(result);
    }
}
