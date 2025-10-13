using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ToDo.Application.DTOs.Auth;
using ToDo.Application.DTOs.Common;
using ToDo.Application.Interfaces.Services;

namespace ToDo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ResponseDto<LoginResponseDto>>> Register([FromBody] RegisterDto registerDto, [FromServices] IValidator<RegisterDto> validator, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(registerDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ResponseDto<LoginResponseDto>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "Validation failed",
                    StatusCode = 400,
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                });
            }

            try
            {
                var result = await _authService.RegisterAsync(registerDto, cancellationToken);

                return StatusCode(201, new ResponseDto<LoginResponseDto>
                {
                    Data = result,
                    IsSuccess = true,
                    Message = "Registered successfully",
                    StatusCode = 201,
                    Errors = null
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ResponseDto<LoginResponseDto>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "Registration failed",
                    StatusCode = 400,
                    Errors = new List<string> { ex.Message }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<LoginResponseDto>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "An unexpected error occurred",
                    StatusCode = 500,
                    Errors = new List<string> { "Internal server error" }
                });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<ResponseDto<LoginResponseDto>>> Login([FromBody] LoginDto loginDto, [FromServices] IValidator<LoginDto> validator, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(loginDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ResponseDto<LoginResponseDto>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "Validation failed",
                    StatusCode = 400,
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                });
            }

            try
            {
                var result = await _authService.LoginAsync(loginDto, cancellationToken);

                return Ok(new ResponseDto<LoginResponseDto>
                {
                    Data = result,
                    IsSuccess = true,
                    Message = "Login successful",
                    StatusCode = 200,
                    Errors = null
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ResponseDto<LoginResponseDto>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ex.Message,
                    StatusCode = 401,
                    Errors = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto<LoginResponseDto>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "An unexpected error occurred",
                    StatusCode = 500,
                    Errors = new List<string> { "Internal server error" }
                });
            }
        }
    }
}