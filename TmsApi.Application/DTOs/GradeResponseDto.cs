namespace TmsApi.Application.Dtos;
public record GradeResponseDto
(
    int Id,
    int StudentId,
    int CourseId,
    int Score
);