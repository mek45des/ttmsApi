using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.Entities;
namespace TmsApi.Services;
using Tms.Api.Dtos;

public interface ICourseService
{
    
    Task<CourseResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
Task<CourseResponseDto> CreateAsync(CreateCourseRequest request, CancellationToken cancellationToken);
Task<bool> CodeExistsAsync(string code, CancellationToken ct);
}