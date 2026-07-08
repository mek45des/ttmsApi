using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TmsApi.Data;
namespace TmsApi.Controllers;
using TmsApi.Entities;
using TmsApi.Services;

[ApiController]
[Route("api/test")]
public class TestController(TmsDbContext context, IStudentService studentService) : ControllerBase
{
    [HttpGet("deferred")]
    public IActionResult TestDeferred()
    {
        Console.WriteLine("\n>>> STEP 1: Building the query object (nodatabase contact)...");
        var query = context.Students.Where(s => s.GPA >= 3.0m);
        Console.WriteLine(">>> STEP 2: Appending a sorting clause...");
        var orderedQuery = query.OrderBy(s => s.Name);
        Console.WriteLine(">>> STEP 3: Materializing query into a C# List...");
        var results = orderedQuery.ToList(); // Execution is triggeredhere
        Console.WriteLine(">>> STEP 4: Materialization finished. List populated.\n");
        return Ok(results);
    }
    private static bool IsHonorRoll(decimal gpa)
    {
        return gpa >= 3.5m;
    }

    [HttpGet]
public async Task<ActionResult<IReadOnlyList<Student>>> GetStudents(
    int page = 1,
    CancellationToken cancellationToken = default)
{
    var students = await studentService.GetStudentsPageAsync(
        page,
        cancellationToken);

        var student = await context.Students.AsNoTracking().ToListAsync(cancellationToken);
foreach (var s in student)
{
// TODO: Query enrollment count for this student inside the loop (use StudentId).
// This should produce 1 + N SQL statements. Count them in the log.
var count = await context.Enrollments
.AsNoTracking()
.CountAsync(e => e.StudentId == s.Id, cancellationToken);
Console.WriteLine($"{s.Name}: {count} enrollments");
}

    return Ok(students);

    
}

    [HttpGet("translation-fail")]
    public async Task<IActionResult> TestTranslationFail()
    {

        try
        {
            var list = await context.Courses
.Select(c => new
{
    c.Title,
    EnrollmentCount = c.Enrollments.Count
})
.OrderByDescending(x => x.EnrollmentCount)
.ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($">>> EXCEPTION CAUGHT: {ex.Message}\n");
            return BadRequest(new { Message = ex.Message });
        }


        try
        {
            var list = await context.Enrollments
.GroupBy(e => e.Course.Title)
.Select(g => new
{
Course = g.Key,
AverageGPA = g.Average(e => e.Student.GPA)
})
.ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($">>> EXCEPTION CAUGHT: {ex.Message}\n");
            return BadRequest(new { Message = ex.Message });
        }
        Console.WriteLine("\n>>> STEP 1: Running non-translatable query...");
        try
        {
            var counts = await context.Students
            .Where(s => s.IsActive && s.GPA >= 3.0m) // EF Core does not know how to map this method to SQL
            .CountAsync();
            return Ok(counts);
        }
        catch (Exception ex)
        {
            Console.WriteLine($">>> EXCEPTION CAUGHT: {ex.Message}\n");
            return BadRequest(new { Message = ex.Message });
        }

    }
}