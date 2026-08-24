using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TmsApi.Application.Enrollments.Commands;
using TmsApi.Application.Enrollments.Queries;
namespace TmsApi.Api.Controllers;

using TmsApi.Api.Hubs;
using Microsoft.AspNetCore.SignalR;
using TmsApi.Application.Hubs;
using TmsApi.Application.Dtos;
using TmsApi.Application.Interface;
[ApiController]
[Route("api/enrollments")]
[ApiVersion("2.0")]
//[ApiVersion("1.0")]
public class EnrollmentsController(IMediator mediator, IEnrollmentService enrollmentService, IHubContext<TmsHub, ITmsHubClient> hubContext) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Enroll(
    EnrollStudentCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.Match<IActionResult>(
        onSuccess: created => CreatedAtAction(
        nameof(GetSchedule),
        new { studentId = created.StudentId },
        created),
        onFailure: error =>
        {
            var status = error.Code switch
            {
                "course_not_found" => StatusCodes.Status404NotFound,
                "course_full" or "already_enrolled" => StatusCodes.
        Status409Conflict,
                _ => StatusCodes.Status400BadRequest
            };
            return Problem(
    statusCode: status,
    title: "Enrollment rejected",
    detail: error.Message,
    type: $"https://tms.local/errors/{error.Code}");
        });

    }

    [HttpGet("{studentId}/schedule")]
    public async Task<IActionResult> GetSchedule(
    int studentId, CancellationToken ct)
    {
        var schedule = await mediator.Send(
        new GetStudentScheduleQuery(studentId), ct);
        return Ok(schedule);
    }
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<EnrollmentResponseDto>), StatusCodes.Status200OK)]
    [EndpointSummary("List enrollments with pagination")]
    [EndpointDescription("Returns a paginated, optionally filtered list of TMS enrollments. Page size is capped at 50.")]
    public async Task<IActionResult> GetEnrollments(
  [FromQuery] PagedRequest request, CancellationToken ct)
    {
        var result = await enrollmentService.GetEnrollmentsAsync(request, ct);
        return Ok(result);
    }

   
    // Your existing approval logic ...
    // After the database commit succeeds, broadcast to all connected Angular clients
}


