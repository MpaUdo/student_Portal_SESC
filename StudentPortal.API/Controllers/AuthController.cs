using Microsoft.AspNetCore.Mvc;
using StudentPortal.Models.Dtos.Requests;
using StudentPortal.Models.Dtos.Responses;
using StudentPortal.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace StudentPortal.API.Controllers
{
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IStudentService _studentService;
        private readonly IAuthService _authService;

        public AuthController(IStudentService studentService, IAuthService authService)
        {
            _studentService = studentService;
            _authService = authService;
        }

        [SwaggerOperation(Summary = "Registers a Student")]
        [HttpPost("register",Name = "register")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Registers a Student")]
        public async Task<IActionResult> Register(RegisterStudentDto register)
        {
            await _studentService.Register(register);
            return Ok();
        }

        [SwaggerOperation(Summary = "Login a Student")]
        [HttpPost("login", Name = "login")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Logs in a Student", Type = typeof(LoggedInUserResponse))]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var response = await _authService.UserLogin(request);
            return Ok(response);
        }
    }
}
