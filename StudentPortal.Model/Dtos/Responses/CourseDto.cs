using StudentPortal.Models.Entities;

namespace StudentPortal.Models.Dtos.Responses;

public class CourseDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Fee { get; set; }
}