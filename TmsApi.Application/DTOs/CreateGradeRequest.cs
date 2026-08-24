using System.ComponentModel.DataAnnotations;
namespace TmsApi.Application.Dtos;
public record CreateGradeRequest
{
    [Range(0, 100, ErrorMessage = "Score must be between 0 and 100.")]
    public required int Score { get; init; }
    public required int StudentId { get; init; }
    public required int CourseId { get; init; }
}