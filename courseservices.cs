using Microsoft.EntityFrameworkCore;
using Tms.Api.Dtos;
using TmsApi.Data;
using TmsApi.Entities;
namespace TmsApi.Services;

public class CourseService(TmsDbContext context, ILogger<CourseService>
logger): ICourseService
{
    public   Task<CourseResponseDto?> GetByIdAsync(int id,
        CancellationToken cancellationToken)
    {
       return context.Courses
.AsNoTracking()
.Where(c => c.Id == id)
.Select(c => new CourseResponseDto(
c.Id, c.Code, c.Title, c.MaxCapacity, c.Enrollments.Count))
.FirstOrDefaultAsync(cancellationToken);
    }
public async Task<CourseResponseDto> CreateAsync(
    CreateCourseRequest request,
    CancellationToken cancellationToken)
{
   var course = new Course
{
Code = request.Code,
Title = request.Title,
MaxCapacity = request.MaxCapacity
};
context.Courses.Add(course);
await context.SaveChangesAsync(cancellationToken);
logger.LogInformation("Created course {CourseId} ({Code})", course.
Id, course.Code);
return (await GetByIdAsync(course.Id, cancellationToken))!;
}
public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken)
    {
        return context.Courses.AsNoTracking().AnyAsync(c => c.Code == code, cancellationToken);
    }
}