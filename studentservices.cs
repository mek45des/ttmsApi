using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.Entities;
namespace TmsApi.Services;


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
