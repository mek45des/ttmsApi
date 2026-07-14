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
    public async Task<PagedResponse<CourseResponseDto>> GetCoursesAsync(
        PagedRequest request, CancellationToken ct)
    {
        IQueryable<Course> query = context.Courses.AsNoTracking();
        if(request.Search is not null)
        {
           query = query.Where(c => EF.Functions.ILike(c.Title, $"%{request.Search}%")
            || EF.Functions.ILike(c.Code, $"%{request.Search}%"));
        }
            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(c => c.Title)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new CourseResponseDto(
                    c.Id, c.Code, c.Title, c.MaxCapacity, c.Enrollments.Count))
                .ToListAsync(ct);
                
               

               return new PagedResponse<CourseResponseDto> { Items = items, TotalCount = totalCount, 
                Page = request.Page, PageSize = request.PageSize };
                throw new NotImplementedException();
        }
    }
