//using Microsoft.EntityFrameworkCore;
//using TmsApi.Infrastructure.Persistence;
using TmsApi.Domain.Entities;
namespace TmsApi.Application.Interface;
using TmsApi.Application.Dtos;

public interface ICourseService
{
    
    Task<CourseResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
Task<CourseResponseDto> CreateAsync(CreateCourseRequest request, CancellationToken cancellationToken);
Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken);
Task<PagedResponse<CourseResponseDto>> GetCoursesAsync(PagedRequest
request, CancellationToken ct);
}