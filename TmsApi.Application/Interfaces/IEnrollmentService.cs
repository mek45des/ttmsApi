namespace TmsApi.Application.Interface;
using TmsApi.Application.Dtos;
using TmsApi.Application.Enrollments.Commands;
using TmsApi.Application.Enrollments.Queries;
using TmsApi.Domain.Entities;

public interface IEnrollmentService
{
Task<EnrollmentResponseDto?> GetByIdAsync(int courseId, int id, CancellationToken cancellationToken);
Task<EnrollmentResponseDto> CreateAsync(int courseId, EnrollStudentRequest request, CancellationToken ct);
Task<List<EnrollmentResponseDto>> GetByCourseAsync(
    int courseId,
    CancellationToken ct);
    
    Task<bool> ExistsAsync(int studentId, string courseCode, CancellationToken ct);
    Task AddAsync(Enrollment enrollment, CancellationToken ct);
    Task<List<Enrollment>> GetByStudentIdAsync(
        int studentId,
        CancellationToken ct);
        Task<PagedResponse<EnrollmentResponseDto>> GetEnrollmentsAsync(PagedRequest
request, CancellationToken ct);
Task<EnrollmentResponseDto?> ApproveAsync(
        int courseId,
        int id,
        CancellationToken ct);
        
}