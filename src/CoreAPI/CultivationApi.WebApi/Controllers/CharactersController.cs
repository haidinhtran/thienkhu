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
}
