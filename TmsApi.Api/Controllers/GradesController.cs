using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
//using TmsApi.Application.Enrollments.Commands;
//using TmsApi.Application.Enrollments.Queries;
namespace TmsApi.Api.Controllers;

using TmsApi.Application.Dtos;
using TmsApi.Application.Interface;
[ApiController]
[Route("api/v{version:apiVersion}/grades")]
[ApiVersion("2.0")]
public class GradesController(IMediator mediator, IGradeService GradeService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<GradeResponseDto>), StatusCodes.Status200OK)]
    [EndpointSummary("List grades with pagination")]
    [EndpointDescription("Returns a paginated, optionally filtered list of TMS grades. Page size is capped at 50.")]
    public async Task<IActionResult> GetGrades(
  [FromQuery] PagedRequest request, CancellationToken ct)
    {
        var result = await GradeService.GetGradesAsync(request, ct);
        return Ok(result);
    }
    [HttpGet("{studentId:int}/{courseId:int}", Name = nameof(GetGradesById))]
     [ProducesResponseType(typeof(GradeResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Get grades by ID")]
    [EndpointDescription("Returns grade details for a specific student. Returns 404 if the enrollment does not exist.")]
    public async Task<IActionResult> GetGradesById(
        int courseId,
        int studentId,
        CancellationToken ct)
    {
        var grades = await GradeService.GetByIdAsync(
            courseId,
            studentId,
            ct);

        if (grades is null)
            return NotFound();

        return Ok(grades);
    }

[HttpPost(Name = nameof(CreateGrade))]
    [ProducesResponseType(typeof(GradeResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [EndpointSummary("Create a new grade")]
    [EndpointDescription("Creates a new grade for the specified student and course.")]
    public async Task<IActionResult> CreateGrade(
        int studentId,
        int courseId,
        CreateGradeRequest request,
        CancellationToken ct)
    {
        var result = await GradeService.CreateAsync(
            studentId,
            courseId,
            request,
            ct);

        return CreatedAtAction(
            nameof(GetGradesById),
            new
            {
                courseId,
                studentId,
                id = result.Id
            },
            result);
    }
}