using Challengify.Entities.Models;
using Challengify.Entities.Models.DataTransferObject;
using Challengify.Entities.Models.DataTransferObject.Response;
using Challengify.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Challengify.Controllers.Endpoints;

/// <summary>
/// Represents the controller for managing challenges.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class ChallengeController(IChallengeService challengeService, IResultService resultService, IFileService fileService) : ControllerBase
{
    private readonly IChallengeService _challengeService = challengeService;
    private readonly IResultService _resultService = resultService;
    private readonly IFileService _fileService = fileService;

    /// <summary>
    /// Retrieves a challenge by its ID.
    /// </summary>
    /// <param name="id">The ID of the challenge to retrieve.</param>
    /// <returns>The challenge with the specified ID, or NotFound if the challenge does not exist.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ChallengeResponseDto>> GetChallengeById(int id)
    {
        var challenge = await _challengeService.GetChallengeResponseDtoAsync(id);
        if (challenge == null)
        {
            return NotFound();
        }
        return Ok(challenge);
    }

    /// <summary>
    /// Creates a new challenge.
    /// </summary>
    /// <param name="challenge">The challenge data.</param>
    /// <returns>The created challenge.</returns>
    /// <remarks>
    /// This endpoint requires the user to be authenticated.
    /// </remarks>
    [HttpPost("create"), Authorize]
    public async Task<ActionResult<ChallengeResponseDto>> CreateChallenge(ChallengeCreationDto challenge)
    {
        try
        {
            int userId = GetUserIdFromToken();
            var newChallenge = await _challengeService.CreateChallengeAsync(challenge, userId);
            return CreatedAtAction("GetChallengeById", new { id = newChallenge.ChallengeId }, new ChallengeResponseDto(newChallenge));
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

    /// <summary>
    /// Retrieves challenges associated with the current user.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> representing the response of the action.</returns>
    /// <remarks>
    /// This endpoint requires the user to be authenticated.
    /// If the user is not authorized, an Unauthorized status code will be returned.
    /// If an error occurs during the retrieval process, a 500 Internal Server Error status code will be returned.
    /// </remarks>
    [HttpGet("user"), Authorize]
    public async Task<ActionResult<List<ChallengeResponseDto>>> GetUserChallenges()
    {
        try
        {
            int userId = GetUserIdFromToken();
            var challenges = await _challengeService.GetUserChallengesAsync(userId);
            if (challenges == null)
            {
                return NotFound();
            }
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

    /// <summary>
    /// Retrieves the results of a challenge with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the challenge.</param>
    /// <returns>An IActionResult representing the HTTP response.</returns>
    [HttpGet("{id}/results")]
    public async Task<ActionResult<List<ResultResponseDto>>> GetChallengeResults(int id)
    {
        try
        {
            List<ResultResponseDto> results = await _resultService.GetResultsByChallengeIdAsync(id);
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

    /// <summary>
    /// Retrieves the last result for a challenge.
    /// </summary>
    /// <param name="id">The ID of the challenge.</param>
    /// <returns>The last result for the challenge, or null if not found.</returns>
    /// <remarks>
    /// This endpoint requires the user to be authenticated.
    /// If the challenge is not found, a 404 Not Found response is returned.
    /// If an error occurs, a 500 Internal Server Error response is returned.
    /// </remarks>
    [HttpGet("{id}/last-result"), Authorize]
    public async Task<ActionResult<ResultResponseDto?>> GetLastResult(int id)
    {
        try
        {
            int userId = GetUserIdFromToken();
            ResultResponseDto? result = await _resultService.GetLastResultByChallengeIdAsync(id, userId);
            return Ok(result);
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

    /// <summary>
    /// Retrieves the result with the specified ID.
    /// </summary>
    /// <param name="resultId">The ID of the result to retrieve.</param>
    /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
    [HttpGet("/get-result/{resultId}")]
    public async Task<ActionResult<ResultResponseDto>> GetResult(int resultId)
    {
        try
        {
            var result = await _resultService.GetResultAsync(resultId);
            if (result == null)
            {
                return NotFound("Result not found");
            }

            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Result not found");
        }
        catch
        {
            return StatusCode(500);
        }
    }

    [HttpGet("{id}/participants"), Authorize]
    public async Task<ActionResult<List<UserResponseDto>>> GetChallengeParticipants(int id)
    {
        try
        {
            Challenge challenge = await _challengeService.GetChallengeAsync(id) ?? throw new KeyNotFoundException("Challenge not found");
            List<UserResponseDto> participants = challenge.Participants.Select(p => new UserResponseDto(p)).ToList();
            return Ok(participants);
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



    /// <summary>
    /// Adds a participant to a challenge.
    /// </summary>
    /// <param name="id">The ID of the challenge.</param>
    /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
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

    /// <summary>
    /// Joins a challenge with the specified join code.
    /// </summary>
    /// <param name="joinCode">The join code of the challenge.</param>
    /// <returns>An IActionResult representing the result of the join operation.</returns>
    /// <remarks>
    /// This method is used to join a challenge by providing the join code. It verifies the user's authorization,
    /// retrieves the user ID from the token, and calls the JoinChallengeAsync method of the challenge service to
    /// join the challenge. If the join code is invalid, a KeyNotFoundException is thrown. If the user is not authorized,
    /// an UnauthorizedAccessException is thrown. If any other error occurs, a 500 Internal Server Error is returned.
    /// </remarks>
    [HttpPut("{joinCode}/join"), Authorize]
    public async Task<IActionResult> JoinChallenge(string joinCode)
    {
        try
        {
            Console.WriteLine(joinCode);
            int userId = GetUserIdFromToken();
            var challenge = await _challengeService.JoinChallengeAsync(joinCode, userId);
            return Ok(challenge);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Challenge not found");
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

    /// <summary>
    /// Adds a result to a challenge.
    /// </summary>
    /// <param name="id">The ID of the challenge.</param>
    /// <param name="result">The result to be added.</param>
    /// <returns>An <see cref="ActionResult<ResultResponseDto>"/> representing the result of the operation.</returns>
    [HttpPut("{id}/add-result"), Authorize]
    public async Task<ActionResult<ResultResponseDto>> AddResult(int id, [FromForm] ResultCreateRequestDto result, [FromForm] IFormFile? file = null)
    {
        try
        {
            int userId = GetUserIdFromToken();
            string customFileName = string.Empty;
            if (file != null)
            {
                customFileName = $"result_{id}_{userId}_{DateTime.Now:yyyyMMddHHmmss}_{file.FileName}";

                var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "files");

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                var filePath = Path.Combine(directoryPath, customFileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

            }

            ResultCreationDto resultCreationDto = new()
            {
                Name = result.Name,
                Description = result.Description,
                UserId = userId,
                ChallengeId = id,
                MediaPath = file != null ? customFileName : ""
            };

            var newResult = await _resultService.CreateResultAsync(resultCreationDto);
            if (file != null)
                await _fileService.UploadFileAsync(customFileName, file.OpenReadStream(), file.ContentType);

            return CreatedAtAction("GetResult", new { resultId = newResult.ResultId }, new ResultResponseDto(newResult));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Challenge not found");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return StatusCode(500);
        }
    }

    [HttpGet("media/{mediaPath}")]
    public async Task<IActionResult> GetMedia(string mediaPath)
    {
        try
        {
            var stream = await _fileService.DownloadFileAsync(mediaPath);
            return File(stream, "image/jpeg");
        }
        catch (KeyNotFoundException)
        {
            return NotFound("File not found");
        }
        catch
        {
            return StatusCode(500);
        }
    }


    /// <summary>
    /// Retrieves the user ID from the token.
    /// </summary>
    /// <returns>The user ID extracted from the token.</returns>
    private int GetUserIdFromToken()
    {
        if (!int.TryParse(User?.FindFirst(c => c.Type.Contains("nameidentifier"))?.Value, out int userId))
        {
            throw new UnauthorizedAccessException("Invalid token");
        }
        return userId;
    }
}