using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentPortal.Models.Configuration.MappingProfiles;
using StudentPortal.Models.Dtos.Requests;
using StudentPortal.Models.Dtos.Responses;
using StudentPortal.Models.Entities;
using StudentPortal.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace StudentPortal.API.Controllers
{
    [Authorize]
    [ApiController]

    public class StudentController : BaseController
    {
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;


        public StudentController(IStudentService studentService, ICourseService courseService)
        {
            _studentService = studentService;
            _courseService = courseService;

        }

        [SwaggerOperation(Summary = "Display Student Profile")]
        [HttpGet("profile")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Get Student Profile",
            Type = typeof(StudentProfileDto))]
        public async Task<IActionResult> GetStudentProfile()
        {
            var response = await _studentService.GetStudentProfile();
            return Ok(response);
        }

        [SwaggerOperation(Summary = "Update Student Profile")]
        [HttpPut("profile")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Update Student Profile",
            Type = typeof(StudentProfileDto))]
        public async Task<IActionResult> UpdateStudentProfile(UpdateStudentProfileDto request)
        {
            await _studentService.UpdateProfile(request);
            return Ok();
        }

        [SwaggerOperation(Summary = "Enroll a Student to course")]
        [HttpPost("course-enrollment/{courseId}")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        public async Task<IActionResult> Enroll(string courseId)
        {
            var response = await _courseService.Enroll(courseId);
            return Ok(response);
        }

        [SwaggerOperation(Summary = "Fetch a Student's Course Enrollment")]
        [HttpGet("course-enrollment")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Get Student Course Enrollment",
            Type = typeof(PagedResponse<StudentEnrollmentDto>))]
        public async Task<IActionResult> GetStudentProfile([FromQuery] StudentEnrollmentRequestDto request)
        {
            var response = await _courseService.GetStudentEnrollment(request);
            return Ok(response);
        }

        [SwaggerOperation(Summary = "Student Graduation Status")]
        [HttpGet("graduation-status")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Get Student graduation status", Type = typeof(bool))]
        public async Task<IActionResult> GetStudentGraduationStatus()
        {
            var response = await _studentService.GetGraduationStatus();
            return Ok(response);
        }
    }
}