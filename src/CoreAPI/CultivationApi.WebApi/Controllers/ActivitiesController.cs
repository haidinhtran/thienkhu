using CultivationApi.Application.DTOs;
using CultivationApi.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CultivationApi.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ActivitiesController : ControllerBase
{
    private readonly IActivitiesService _activitiesService;

    public ActivitiesController(IActivitiesService activitiesService)
    {
        _activitiesService = activitiesService;
    }

    [HttpPost("explore")]
    public async Task<ActionResult<ExplorationEventDto>> StartExploration(
        [FromBody] StartExplorationRequestDto request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.DiscordId) || string.IsNullOrWhiteSpace(request.ServerId) || string.IsNullOrWhiteSpace(request.LocationId))
        {
            return BadRequest(new ProblemDetails 
            { 
                Title = "Validation Error", 
                Detail = "discordId, serverId, and locationId are required." 
            });
        }

        var eventDto = await _activitiesService.StartExplorationAsync(request.DiscordId, request.ServerId, request.LocationId, ct);
        return Ok(eventDto);
    }

    [HttpPost("explore/choice")]
    public async Task<ActionResult<ExplorationResultDto>> SubmitExplorationChoice(
        [FromBody] ExplorationChoiceRequestDto request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.DiscordId) || string.IsNullOrWhiteSpace(request.ServerId) || string.IsNullOrWhiteSpace(request.ChoiceId))
        {
            return BadRequest(new ProblemDetails 
            { 
                Title = "Validation Error", 
                Detail = "discordId, serverId, and choiceId are required." 
            });
        }

        var resultDto = await _activitiesService.SubmitExplorationChoiceAsync(request, ct);
        return Ok(resultDto);
    }
}
