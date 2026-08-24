using System;
namespace TmsApi.Domain.Entities;
public class Enrollment
{
public int Id { get; set; }
public int StudentId { get; set; }
public int CourseId { get; set; }
public decimal? Grade { get; set; } // Nullable, as student may be currently enrolled
public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
 // public string StudentName { get; set; } = string.Empty;
  //  public string CourseName { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
// Navigation properties back to entities
public Student Student { get; set; } = null!;
public Course Course { get; set; } = null!;
 public int Year { get; set; } 
}