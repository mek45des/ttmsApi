using TmsApi.Application.Dtos;
using TmsApi.Infrastructure.Persistence;
namespace TmsApi.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using TmsApi.Application.Interface;
using TmsApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class EnrollmentService (TmsDbContext context, ILogger<EnrollmentService> logger): IEnrollmentService
{
  public  Task<EnrollmentResponseDto?> GetByIdAsync(int courseId, int
id, CancellationToken cancellationToken) {
return context.Enrollments
.AsNoTracking()
.Where(e => e.Id == id && e.CourseId == courseId)
.Select(e => new EnrollmentResponseDto(e.Id, e.CourseId, e.
StudentId, e.EnrolledAt))
.FirstOrDefaultAsync(cancellationToken);
}
public async Task<EnrollmentResponseDto> CreateAsync(int courseId,
EnrollStudentRequest request, CancellationToken cancellationToken)
    {
      var enrollment = new Enrollment
      {
          
        CourseId = courseId,
        StudentId = request.StudentId,
        EnrolledAt = DateTime.UtcNow
      };
      context.Enrollments.Add(enrollment);
      await context.SaveChangesAsync(cancellationToken);
      
      logger.LogInformation(
    "Student {StudentId} enrolled in Course {CourseId}.",
    request.StudentId,
    courseId);

      return (await GetByIdAsync(courseId, enrollment.Id, cancellationToken))!;
      throw new NotImplementedException();
    }
    public async Task<List<EnrollmentResponseDto>> GetByCourseAsync(
    int courseId,
    CancellationToken ct)
{
    return await context.Enrollments
        .Where(e => e.CourseId == courseId)
        .Select(e => new EnrollmentResponseDto
        (
            e.Id,
             e.StudentId,
            e.CourseId,
            e.EnrolledAt
        ))
        .ToListAsync(ct);
}
}


