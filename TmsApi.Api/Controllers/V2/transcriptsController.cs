using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TmsApi.Application.Enrollments.Queries;
namespace TmsApi.Api.Controllers.V2;
[ApiController]
[Route("api/v2/transcripts")]
public class TranscriptsController : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting("transcripts")]
    public async Task<IActionResult> RequestTranscript([FromBody] object? _)
    {
        // Stub: Exercise 5 swaps this for enqueue + 202 + Location.
        return Ok();
    }
 
}