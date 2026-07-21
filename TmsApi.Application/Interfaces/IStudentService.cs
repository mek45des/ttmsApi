//using Microsoft.EntityFrameworkCore;
//using TmsApi.Infrastructure.Persistence;
using TmsApi.Domain.Entities;
namespace TmsApi.Application.Interface;
public interface IStudentService
{
    Task<IReadOnlyList<Student>> GetStudentsPageAsync(
        int page,
        CancellationToken cancellationToken);
        
}