using Microsoft.EntityFrameworkCore;
using TmsApi.Infrastructure.Persistence;
using TmsApi.Domain.Entities;
namespace TmsApi.Infrastructure.Services;
using TmsApi.Application.Interface;

public class StudentService(TmsDbContext context): IStudentService
{
    public  async Task<IReadOnlyList<Student>> GetStudentsPageAsync(
        int page,
        CancellationToken cancellationToken)
    {
        const int pageSize = 20;

        return await context.Students
            .OrderBy(s => s.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
    
    
}
