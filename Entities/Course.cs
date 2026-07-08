using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TmsApi.Entities;
public class Course
{
public int Id { get; set; }
// surrogate primary key — internal, used by foreign keys
public required string Code { get; set; } // natural key — human-readable (uniqueness configured in Session 2)
public required string Title { get; set; }
public int MaxCapacity { get; set; }
// Navigation property for many-to-many relationship
public ICollection<Enrollment> Enrollments { get; set; } = [];
public void Configure(EntityTypeBuilder<Course> builder)
{
builder.HasKey(c => c.Id);
builder.Property(c => c.Code).IsRequired().HasMaxLength(10);
builder.Property(c => c.Title).IsRequired().HasMaxLength(200);
builder.HasIndex(c => c.Code).IsUnique();
builder.HasMany(c => c.Enrollments).WithOne(e => e.Course).HasForeignKey(e => e.CourseId);
}
}