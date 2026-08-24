using MediatR;
using TmsApi.Application.Common;
using TmsApi.Application.Interface;
using TmsApi.Domain.Entities;
namespace TmsApi.Application.Enrollments.Commands;

public class EnrollStudentHandler(
IEnrollmentService enrollmentService,
ICachedCourseService cachedService)
: IRequestHandler<EnrollStudentCommand, Result<EnrollmentCreated, EnrollmentError>>
{
    public async Task<Result<EnrollmentCreated, EnrollmentError>> Handle(
    EnrollStudentCommand command, CancellationToken ct)
    {
        var course = await cachedService.GetCourseAsync(command.CourseCode, ct);
        if (course is null)
            return Result<EnrollmentCreated, EnrollmentError>.Failure(
            EnrollmentError.CourseNotFound(command.CourseCode));

        if (course.EnrollmentCount >= course.MaxCapacity)
            return Result<EnrollmentCreated, EnrollmentError>.Failure(
            EnrollmentError.CourseFull(course.Title, course.MaxCapacity));

        if (await enrollmentService.ExistsAsync(command.StudentId, command.CourseCode, ct))
            return Result<EnrollmentCreated, EnrollmentError>.Failure(
            EnrollmentError.AlreadyEnrolled(command.StudentId, command.CourseCode));
        var enrollment = new Enrollment
        {
            StudentId = command.StudentId,
            CourseId = course.Id,
            Status = "Pending",
            EnrolledAt = DateTime.UtcNow
        };
        await enrollmentService.AddAsync(enrollment, ct);
        //await cachedService.InvalidateCourseCacheAsync(ct);
        return Result<EnrollmentCreated, EnrollmentError>.Success(
        new EnrollmentCreated(enrollment.Id, enrollment.StudentId,
        course.Code));
        
        
    }

}