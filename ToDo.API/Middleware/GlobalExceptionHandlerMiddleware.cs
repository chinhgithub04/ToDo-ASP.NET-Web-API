using ToDo.Application.DTOs.Common;

namespace ToDo.API.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex, StatusCodes.Status401Unauthorized, "Unauthorized");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation attempt: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex, StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex, StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception, int statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new ResponseDto<object>
            {
                Data = null,
                IsSuccess = false,
                Message = message,
                StatusCode = statusCode,
                Errors = statusCode == 500 ? new List<string> { "Internal server error" } : new List<string> { exception.Message }
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
