using Microsoft.EntityFrameworkCore;
using StudentPortal.Models.Entities;

namespace StudentPortal.Data.Context;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options):
        base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Student> Students { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Course>(b =>
        {
            b.HasIndex(c => c.Title)
                .IsUnique();

            b.Property(c => c.Title)
                .HasMaxLength(50);

            b.Property(c => c.Description)
                .HasMaxLength(1000);

            b.Property(c => c.Fee)
                .HasPrecision(18, 2);
        });


        builder.Entity<Student>(b =>
        {
            b.HasIndex(c => c.Username)
                .IsUnique();

            b.HasIndex(c => c.Email)
                .IsUnique();

            b.Property(c => c.Username)
                .HasMaxLength(50);

            b.Property(c => c.Firstname)
                .HasMaxLength(50);

            b.Property(c => c.Surname)
                .HasMaxLength(50);

            b.Property(c => c.StudentId)
                .HasMaxLength(50);

        });

        builder.Entity<StudentEnrollment>(b =>
        {
            b.HasKey(se => new { se.StudentId, se.CourseId });

            b.HasOne(se => se.Student)
                .WithMany(s => s.StudentEnrollments)
                .HasForeignKey(se => se.StudentId);
                
            b.HasOne(se => se.Course)
                .WithMany(c => c.StudentEnrollments)
                .HasForeignKey(se => se.CourseId);
        });
            
        base.OnModelCreating(builder);
    }
}