using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.Entities;
public interface IStudentService
{
    Task<IReadOnlyList<Student>> GetStudentsPageAsync(
        int page,
        CancellationToken cancellationToken);
        
}