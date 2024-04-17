namespace StudentPortal.Models.Dtos.Responses;

public class StudentEnrollmentDto
{
    public string StudentId { get; set; }
    public string CourseId { get; set; }
    public CourseDto Course { get; set; }
    public string PaymentStatus { get; set; }
    public string InvoiceReference { get; set; }
}