using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Identity;
using StudentPortal.Models.Dtos.Requests;
using StudentPortal.Models.Dtos.Responses;

namespace StudentPortal.Services.Interfaces;

public interface ICourseService
{
    Task<PagedResponse<CourseDto>> GetCourses(CoursesRequest request);
    Task<CourseDto> GetCourse(string courseId);
    Task<string> Enroll(string courseId);
    Task<PagedResponse<StudentEnrollmentDto>> GetStudentEnrollment(StudentEnrollmentRequestDto request);
}