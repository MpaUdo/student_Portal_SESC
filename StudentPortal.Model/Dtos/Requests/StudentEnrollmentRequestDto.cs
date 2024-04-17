using StudentPortal.Models.Dtos.Responses;

namespace StudentPortal.Models.Dtos.Requests;

public class StudentEnrollmentRequestDto  : RequestParameters
{
    public string? StudentId { get; set; }
   
}