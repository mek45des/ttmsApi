using TmsApi.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using TmsApi.Application.Interface;
public class EnrollmentWorker
{
    private readonly IServiceScopeFactory _scopeFactory;

    public EnrollmentWorker(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public void ProcessBatch()
    {
        using var scope = _scopeFactory.CreateScope();
        var enrollmentService = scope.ServiceProvider.GetRequiredService<IEnrollmentService>();
        //var enrollments = enrollmentService.GetAllAsync().GetAwaiter().GetResult();
    }
}
