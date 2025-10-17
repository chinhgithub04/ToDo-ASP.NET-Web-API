using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDo.Application.DTOs.Common;

namespace ToDo.API.Controllers
{
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected string GetUserIdFromClaims()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token claims");
            }

            return userId;
        }

        protected ActionResult<ResponseDto<T>> ForbiddenResponse<T>(string message = "You don't have permission to access this resource")
        {
            return StatusCode(403, new ResponseDto<T>
            {
                Data = default,
                IsSuccess = false,
                Message = message,
                StatusCode = 403,
                Errors = null
            });
        }

        protected ActionResult<ResponseDto<T>> NotFoundResponse<T>(string message = "Resource not found")
        {
            return NotFound(new ResponseDto<T>
            {
                Data = default,
                IsSuccess = false,
                Message = message,
                StatusCode = 404,
                Errors = null
            });
        }

        protected ActionResult<ResponseDto<T>> BadRequestResponse<T>(string message, List<string> errors = null)
        {
            return BadRequest(new ResponseDto<T>
            {
                Data = default,
                IsSuccess = false,
                Message = message,
                StatusCode = 400,
                Errors = errors ?? new List<string>()
            });
        }

        protected ActionResult<ResponseDto<T>> OkResponse<T>(T data, string message = "Success")
        {
            return Ok(new ResponseDto<T>
            {
                Data = data,
                IsSuccess = true,
                Message = message,
                StatusCode = 200,
                Errors = null
            });
        }

        protected (int pageNumber, int pageSize, string? error) ValidatePagination(int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
                return (pageNumber, pageSize, "Page number must be at least 1");

            if (pageSize < 1 || pageSize > 100)
                return (pageNumber, pageSize, "Page size must be between 1 and 100");

            return (pageNumber, pageSize, null);
        }
    }
}