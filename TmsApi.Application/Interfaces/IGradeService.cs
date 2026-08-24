namespace TmsApi.Application.Interface;
using TmsApi.Application.Dtos;
public interface IGradeService
{
    Task<GradeResponseDto?> GetByIdAsync (int StudentId , int CourseId, CancellationToken cancellationToken);
     Task<PagedResponse<GradeResponseDto>> GetGradesAsync(PagedRequest
request, CancellationToken ct);
    Task<GradeResponseDto> CreateAsync(int studentId,int courseId, CreateGradeRequest request, CancellationToken cancellationToken);
}