using Microsoft.AspNetCore.Mvc;
using Tms.Api.Dtos;

using TmsApi.Services;
namespace Tms.Api.Controllers;
using TmsApi.Entities;

[ApiController]
[Route("api/courses/{courseId:int}/enrollments")]
[Tags("Enrollments")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class EnrollmentsController(ICourseService courseService,IEnrollmentService enrollmentService) : ControllerBase
{
// GET /api/enrollments returns all enrollment records
[HttpGet("{id:int}", Name = nameof(GetEnrollment))]
[ProducesResponseType(typeof(EnrollmentResponseDto), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[EndpointSummary("Get one enrolment for a course")]
public async Task<IActionResult> GetEnrollment(int courseId, int id, CancellationToken cancellationToken)
{
var enrollment = await enrollmentService.GetByIdAsync(courseId, id, cancellationToken);
return enrollment is not null ? Ok(enrollment) : NotFound();
}
[HttpPost]
[ProducesResponseType(typeof(EnrollmentResponseDto), StatusCodes.Status201Created)]
[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.
Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
[EndpointSummary("Enrol a student in a course")]
[EndpointDescription("Returns 404 if the course does not exist, 409if the course has reached MaxCapacity.")]
public async Task<IActionResult> EnrollStudent(int courseId, EnrollStudentRequest request, CancellationToken cancellationToken){
    var course = await courseService.GetByIdAsync(courseId, cancellationToken);

    if (course==  null)
    {
        return NotFound();
    }
   var enrollment = await enrollmentService.CreateAsync(courseId, request, cancellationToken);
    if(course.EnrollmentCount >= course.MaxCapacity)
        {
            return Conflict(new ProblemDetails { 
                Title = "Course is full",
                Detail = $"Course '{course.Title}' has reached its maximum capacity of {course.MaxCapacity}.",
                Status = StatusCodes.Status409Conflict });
        }
        else
        {
           
            return CreatedAtAction(nameof(GetEnrollment),
            new { courseId, id = enrollment.Id }, enrollment);
            throw new NotImplementedException();
        }
}
  [HttpGet(Name = "ListCourseEnrollments")]
  [ProducesResponseType(typeof(IReadOnlyList<EnrollmentResponseDto>),
StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[EndpointSummary("List enrolments for a course")]
public async Task<IActionResult> GetEnrollments(int courseId, CancellationToken ct)
{
var course = await courseService.GetByIdAsync(courseId, ct);
if (course == null)
{
    return NotFound();
}
return Ok(await enrollmentService.GetByCourseAsync(courseId, ct));
throw new NotImplementedException();
}

}

