using Microsoft.AspNetCore.Mvc;
using StaffManagementSystem.Application.DTOs.Response;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StaffManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult Success<T>(T data, string message = "Success")
       => Ok(new ApiResponse<T>(true, message, data));

        protected IActionResult Error(string message, int statusCode = 400)
        => StatusCode(statusCode, new ApiResponse<string>(false, message));

        protected IActionResult ValidationError(Dictionary<string, string[]> errors, string message = "Validation failed")
        => BadRequest(new ApiResponse<string>(false, message, null, errors));
    }
}
