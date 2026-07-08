using Microsoft.AspNetCore.Mvc;
using Tms.Api.Dtos;

using TmsApi.Services;
namespace Tms.Api.Controllers;
using TmsApi.Entities;

[ApiController]
[Route("api/courses/{courseId:int}/enrollments")]
public class EnrollmentsController(ICourseService courseService,IEnrollmentService enrollmentService) : ControllerBase
{
// GET /api/enrollments returns all enrollment records
[HttpGet("{id:int}", Name = nameof(GetEnrollment))]
public async Task<IActionResult> GetEnrollment(int courseId, int id, CancellationToken cancellationToken)
{
var enrollment = await enrollmentService.GetByIdAsync(courseId, id, cancellationToken);
return enrollment is not null ? Ok(enrollment) : NotFound();
}
[HttpPost]
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
  

}

