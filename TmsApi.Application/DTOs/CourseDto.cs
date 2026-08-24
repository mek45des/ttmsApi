namespace TmsApi.Application.Dtos;

public record CourseDto(
    int Id,
    string Title,
    string Code,
    int MaxCapacity,
    int EnrolledStudents,
    int EnrollmentsCount
);