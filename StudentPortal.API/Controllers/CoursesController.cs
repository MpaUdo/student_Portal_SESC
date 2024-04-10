using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentPortal.Models.Dtos.Requests;
using StudentPortal.Models.Dtos.Responses;
using StudentPortal.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace StudentPortal.API.Controllers;

[ApiController]

[Authorize]
public class CoursesController : BaseController
{

    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }
    [SwaggerOperation(Summary = "Fetch all Courses")]
    [HttpGet]
    [SwaggerResponse(StatusCodes.Status200OK, Description = "Get Courses",
        Type = typeof(PagedResponse<CourseDto>))]
    public async Task<IActionResult> GetCourses([FromQuery] CoursesRequest request)
    {
        var response = await _courseService.GetCourses(request);
        return Ok(response);
    }

    [SwaggerOperation(Summary = "Fetch a single course")]
    [HttpGet("{courseId}")]
    [SwaggerResponse(StatusCodes.Status200OK, Description = "Get Courses",
        Type = typeof(CourseDto))]
    public async Task<IActionResult> GetCourses(string courseId)
    {
        var response = await _courseService.GetCourse(courseId);
        return Ok(response);
    }
}