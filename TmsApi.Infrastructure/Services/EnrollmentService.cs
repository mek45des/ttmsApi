using TmsApi.Application.Dtos;
using TmsApi.Infrastructure.Persistence;
namespace TmsApi.Infrastructure.Services;

using Microsoft.Extensions.Logging;
using TmsApi.Application.Interface;
using TmsApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

public class EnrollmentService(TmsDbContext context, ILogger<EnrollmentService> logger) : IEnrollmentService
{
  public Task<EnrollmentResponseDto?> GetByIdAsync(int courseId, int
id, CancellationToken cancellationToken)
  {
    return context.Enrollments
    .AsNoTracking()
    .Where(e => e.Id == id && e.CourseId == courseId)
    .Select(e => new EnrollmentResponseDto(
      e.Id,
      e.CourseId,
      e.StudentId,
      e.Student.Name,
      e.Course.Title,
      e.Status,
      e.EnrolledAt))
    .FirstOrDefaultAsync(cancellationToken);
  }
  public async Task<EnrollmentResponseDto> CreateAsync(int courseId,
  EnrollStudentRequest request, CancellationToken cancellationToken)
  {
    var enrollment = new Enrollment
    {

      CourseId = courseId,
      StudentId = request.StudentId,
      EnrolledAt = DateTime.UtcNow,
      Status = "Pending"
    };
    context.Enrollments.Add(enrollment);
    await context.SaveChangesAsync(cancellationToken);

    logger.LogInformation(
  "Created enrollment {EnrollmentId} for Course {CourseId} and Student {StudentId}",
  request.StudentId,
  enrollment.Id,
  courseId);

    return (await GetByIdAsync(courseId, enrollment.Id, cancellationToken))!;
    //throw new NotImplementedException();
  }

  public async Task<List<EnrollmentResponseDto>> GetByCourseAsync(

  int courseId,
  CancellationToken ct)
  {
  
   
    return await context.Enrollments
        .Where(e => e.CourseId == courseId)
        
        .Select(e =>  new EnrollmentResponseDto
        (
            e.Id,
             e.StudentId,
            e.CourseId,
            e.Student.Name,
            e.Course.Title,
            e.Status,
            e.EnrolledAt
            
        ))
        .ToListAsync(ct);
  
  }
  public async Task<bool> ExistsAsync(int studentId, string courseCode, CancellationToken ct)
  {
    return await context.Enrollments
    .Include(e => e.Course)
    .Include(e => e.Student)
    .AnyAsync(
        e => e.StudentId == studentId &&
             e.Course.Code == courseCode,
        ct);
  }

  public async Task AddAsync(Enrollment enrollment, CancellationToken ct)
  {
    context.Enrollments.Add(enrollment);
    await context.SaveChangesAsync(ct);
  }
  public async Task<List<Enrollment>> GetByStudentIdAsync(
   int studentId,
   CancellationToken ct)
  {
    return await context.Enrollments
    .Where(e => e.StudentId == studentId)
    .ToListAsync(ct);
  }
  public async Task<PagedResponse<EnrollmentResponseDto>> GetEnrollmentsAsync(
        PagedRequest request, CancellationToken ct)
  {
    IQueryable<Enrollment> query = context.Enrollments.AsNoTracking();
    if (request.Search is not null)
    {
      if (int.TryParse(request.Search, out var searchId))
      {
        query = query.Where(c =>
            c.CourseId == searchId ||
            c.StudentId == searchId
        );
      }
    }
    var totalCount = await query.CountAsync(ct);
    var items = await query
        .OrderBy(c => c.StudentId)
        .Skip((request.Page - 1) * request.PageSize)
        .Take(request.PageSize)
        .Select(c => new EnrollmentResponseDto(
            c.Id, c.CourseId, c.StudentId, c.Student.Name, c.Course.Title,c.Status, c.EnrolledAt ))
        .ToListAsync(ct);
    return new PagedResponse<EnrollmentResponseDto>
    {
      Items = items,
      TotalCount = totalCount,
      Page = request.Page,
      PageSize = request.PageSize
    };
    //throw new NotImplementedException();
  }
  public async Task<EnrollmentResponseDto?> ApproveAsync(
       int courseId,
       int id,
       CancellationToken ct)
  {
    var enrollment = await context.Enrollments
        .FirstOrDefaultAsync(e => e.Id == id && e.CourseId == courseId, ct);

    if (enrollment is null)
      return null;

    if (enrollment.Status == "Approved")
      return null;

    enrollment.Status = "Approved";
    await context.SaveChangesAsync(ct);

    logger.LogInformation(
        "Approved enrollment {EnrollmentId} for Course {CourseId}",
        enrollment.Id,
        courseId);

    return await GetByIdAsync(courseId, id, ct);
  }
}


