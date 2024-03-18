using Challengify.Entities.Models.DataTransferObject;
using Challengify.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ChallengeController : ControllerBase
{
    private readonly IChallengeService _challengeService;

    public ChallengeController(IChallengeService challengeService)
    {
        _challengeService = challengeService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetChallengeById(int id)
    {
        var challenge = await _challengeService.GetChallengeAsync(id);
        if (challenge == null)
        {
            return NotFound();
        }
        return Ok(challenge);
    }

    [HttpPost("create"), Authorize]
    public async Task<IActionResult> CreateChallenge(ChallengeCreationDto challenge)
    {
        try
        {
            int userId = GetUserIdFromToken();
            var newChallenge = await _challengeService.CreateChallengeAsync(challenge, userId);
            return CreatedAtAction("GetChallengeById", new { id = newChallenge.ChallengeId }, newChallenge);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch
        {
            return StatusCode(500);
        }
    }

    [HttpGet("user"), Authorize]
    public async Task<IActionResult> GetUserChallenges()
    {
        try
        {
            int userId = GetUserIdFromToken();
            var challenges = await _challengeService.GetUserChallengesAsync(userId);
            return Ok(challenges);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch
        {
            return StatusCode(500);
        }
    }

    [HttpGet("{id}/results")]
    public async Task<IActionResult> GetChallengeResults(int id)
    {
        try
        {
            var results = await _challengeService.GetChallengeResultsAsync(id);
            return Ok(results);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Challenge not found");
        }
        catch
        {
            return StatusCode(500);
        }
    }

    [HttpPut("{id}/add-participant"), Authorize]
    public async Task<IActionResult> AddParticipant(int id)
    {
        try
        {
            int userId = GetUserIdFromToken();
            var challenge = await _challengeService.AddParticipantAsync(id, userId);
            return Ok(challenge);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch
        {
            return StatusCode(500);
        }
    }

    private int GetUserIdFromToken()
    {
        if (!int.TryParse(User?.FindFirst(c => c.Type.Contains("nameidentifier"))?.Value, out int userId))
        {
            throw new UnauthorizedAccessException("Invalid token");
        }
        return userId;
    }
}