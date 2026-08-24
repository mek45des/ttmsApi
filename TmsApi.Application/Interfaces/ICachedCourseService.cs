namespace TmsApi.Application.Interface;
using TmsApi.Application.Dtos;
public interface ICachedCourseService
{
     Task<CourseResponseDto> GetCourseAsync(string code, CancellationToken ct);
     Task<List<CourseResponseDto>> GetAllCoursesAsync(CancellationToken ct);
     // Task<List<CourseDto>> GetAllAsync(CancellationToken ct);
     Task InvalidateCourseCacheAsync(CancellationToken ct);
     //Task<CourseResponseDto> GetByCodeAsync(string code, CancellationToken ct);
    
}