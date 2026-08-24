using MediatR;

namespace TmsApi.Application.Enrollments.Queries.SearchCoursesQuery;

public record SearchCoursesQuery(string? Term)
    : IRequest<IEnumerable<object>>;