namespace TmsApi.Infrastructure.Services;
using TmsApi.Application.Interface;
using Microsoft.Extensions.Logging;
using TmsApi.Infrastructure.Persistence;
using TmsApi.Application.Dtos;
using TmsApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
public class GradeService(TmsDbContext context, ILogger<GradeService> logger) : IGradeService
{
  public Task<GradeResponseDto?> GetByIdAsync(int studentId, int
courseId, CancellationToken cancellationToken)
  {
    return context.GradePayLoads
    .AsNoTracking()
    .Where(e => e.StudentId == studentId && e.CourseId == courseId)
    .Select(e => new GradeResponseDto(
        e.Id,
      e.CourseId,
      e.StudentId,
      e.Score))
    .FirstOrDefaultAsync(cancellationToken);
  }

   public async Task<PagedResponse<GradeResponseDto>> GetGradesAsync(
        PagedRequest request, CancellationToken ct)
  {
    IQueryable<GradePayLoad> query = context.GradePayLoads.AsNoTracking();
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
        .Select(c => new GradeResponseDto(
          c.Id,   c.StudentId,  c.CourseId, c.Score ))
        .ToListAsync(ct);
    return new PagedResponse<GradeResponseDto>
    {
      Items = items,
      TotalCount = totalCount,
      Page = request.Page,
      PageSize = request.PageSize
    };
    //throw new NotImplementedException();
  }
  public async Task<GradeResponseDto> CreateAsync(int studentId,int courseId,  CreateGradeRequest request, CancellationToken cancellationToken)
  {
    var grades = new GradePayLoad
    {

      StudentId = studentId,
      CourseId = courseId,
     Score = request.Score
    };
    context.GradePayLoads.Add(grades);
    await context.SaveChangesAsync(cancellationToken);

    logger.LogInformation(
  "Created grade {GradeId} for Course {CourseId} and Student {StudentId}",
  //request.Score,
  grades.Id,
  courseId,
  studentId);

    return (await GetByIdAsync(studentId, courseId,  cancellationToken))!;
    //throw new NotImplementedException();
  }

}