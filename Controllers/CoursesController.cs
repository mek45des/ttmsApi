using Microsoft.AspNetCore.Mvc;
//using TmsApi.Entities;
using TmsApi.Services;
using Tms.Api.Dtos;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Components.Web;
namespace TmsApi.Controllers;
[ApiController]
[Route("api/courses")]
public class CoursesController(ICourseService courseService) : ControllerBase
{
[HttpGet("{id:int}", Name = nameof(GetCourseById))]
public async Task<IActionResult> GetCourseById(int id, CancellationToken cancellationToken)
{


    var course = await courseService.GetByIdAsync(id, cancellationToken);

    if (course == null)
    {
        return NotFound();
    }

    return Ok(course);
    throw new NotImplementedException();
}


[HttpPost]
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
