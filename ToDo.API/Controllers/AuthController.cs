using Microsoft.AspNetCore.Mvc;
using ToDo.Application.DTOs.Auth;
using ToDo.Application.DTOs.Common;
using ToDo.Application.Interfaces.Services;

namespace ToDo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ResponseDto<LoginResponseDto>>> Register([FromBody] RegisterDto registerDto, CancellationToken cancellationToken)
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

        [HttpPost("login")]
        public async Task<ActionResult<ResponseDto<LoginResponseDto>>> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken)
        {
            var result = await _authService.LoginAsync(loginDto, cancellationToken);

            return OkResponse(result, "Login successfully");

        }
    }
}