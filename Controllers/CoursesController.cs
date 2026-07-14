using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
//using TmsApi.Entities;
using TmsApi.Services;
using Tms.Api.Dtos;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.VisualBasic;
using TmsApi.Entities;
namespace TmsApi.Controllers;
[ApiController]
[Route("api/courses")]
[Tags("Courses")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), 
StatusCodes.Status500InternalServerError)]
public class CoursesController(ICourseService courseService, LinkGenerator linkGenerator) : ControllerBase
{
[HttpGet("{id:int}", Name = nameof(GetCourseById))]
[ProducesResponseType(typeof(CourseDetailDto), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[EndpointSummary("Get a course by ID")]
[EndpointDescription("Returns course details with HATEOAS links. Returns 404 if the course does not exist.")]
public async Task<IActionResult> GetCourseById(int id, CancellationToken cancellationToken)
{
    var course = await courseService.GetByIdAsync(id, cancellationToken);

    if (course == null)
        {
            return NotFound();
        }
        //linkgenerator//
var courseLink=linkGenerator.GetPathByName(HttpContext, nameof(GetCourseById), new { id = course.Id });
var enrollLink=linkGenerator.GetPathByAction(HttpContext, action: "GetEnrollments",
controller:"Enrollments", values: new { courseId = course.Id });

var links=new List<LinkDto>
{
  
    new LinkDto( courseLink,  "self", "GET" ),
    new LinkDto( courseLink, "update", "PUT" ),
    new LinkDto( courseLink, "delete", "DELETE" ),
    new LinkDto( enrollLink, "enrollments",  "GET" )

};
  if (course.EnrollmentCount<course.MaxCapacity){
            new LinkDto(enrollLink, "enroll", "POST");
        }
        var detailDtos=new CourseDetailDto
        {
            Id=course.Id,
            Code=course.Code,
            Title=course.Title,
            MaxCapacity=course.MaxCapacity,
            EnrollmentCount=course.EnrollmentCount,
            Links=links
        };
        return Ok(detailDtos);
    throw new NotImplementedException();
}

[HttpGet]
[ProducesResponseType(typeof(PagedResponse<CourseResponseDto>), StatusCodes.Status200OK)]
[EndpointSummary("List courses with pagination")]
[EndpointDescription("Returns a paginated, optionally filtered listof TMS courses. PageSizis capped at 50.")]
public async Task<IActionResult> GetCourses(
[FromQuery] PagedRequest request, CancellationToken ct)
{
var result = await courseService.GetCoursesAsync(request, ct);
return Ok(result);
}

[HttpPost]
[ProducesResponseType(typeof(CourseResponseDto), StatusCodes.Status201Created)]
[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.
Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
[EndpointSummary("Create a new course")]
[EndpointDescription("Creates a course with a unique code. Returns409 if the course code already exists.")]
public async Task<IActionResult> CodeExistsAsync(CreateCourseRequest request, CancellationToken cancellationToken)
{
 if(await courseService.CodeExistsAsync(request.Code, cancellationToken)){
return Conflict(new ProblemDetails
{
    Title = "Course code already exists",
Detail = $"A course with code '{request.Code}' is already registered.",
Status = StatusCodes.Status409Conflict
});
 }
//If it returns true, return Conflict(new ProblemDetails{ ... }) with: 
//You do not need a try/catch the framework's ProblemDetals middleware handles unhandled exceptions.
var result = await courseService.CreateAsync(request, cancellationToken);
return 
CreatedAtAction(nameof(GetCourseById), 
new { id = result.Id}, result);
}
}
