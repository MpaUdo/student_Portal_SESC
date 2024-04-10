using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using StudentPortal.Models.Dtos.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace StudentPortal.API.Controllers
{
    [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, Description = "User is forbidden", Type = typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Description = "User is unauthorized", Type = typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
       
        public BaseController()
        {

        }


        [ApiExplorerSettings(IgnoreApi = true)]
        public override OkObjectResult Ok(object value)
        {
            SuccessResponse response = new()
            {
                Data = value,
                Success = true
            };

            return base.Ok(response);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public new OkObjectResult Ok()
        {
            SuccessResponse response = new()
            {
                Success = true
            };

            return base.Ok(response);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public NotFoundObjectResult NotFound()
        {
            SuccessResponse response = new()
            {
                Success = false,
            };

            return base.NotFound(response);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public OkObjectResult Invalid()
        {
            SuccessResponse response = new()
            {
               
                Success = false
            };

            /*return invalid */
            return base.Ok(response);

        }

        

        [ApiExplorerSettings(IgnoreApi = true)]
        public string GetUserId()
        {
            string userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public OkObjectResult Error()
        {
            SuccessResponse response = new()
            {
                Success = false
            };

            return base.Ok(response);
        }
    }
}
