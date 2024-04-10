using System.Security.Principal;

namespace StudentPortal.Models.Entities;

public class StudentEnrollment : BaseEntity
{
    public string StudentId { get; set; }
    public Student Student { get; set; }

    public string CourseId { get; set; }
    public Course Course { get; set; }

    public PaymentStatus PaymentStatus { get; set; }
    public string InvoiceReference { get; set; }
}

public enum PaymentStatus
{
    Pending,
    Paid
}