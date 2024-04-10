using StudentPortal.Models.Entities;

namespace StudentPortal.Models.Dtos.Requests;

public class CoursesRequest : RequestParameters
{
    public CoursesRequest()
    {
        OrderBy = "Title";
    }

    public string? StudentId { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
}