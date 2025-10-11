using ToDo.Application.DTOs.Auth;

namespace ToDo.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken);
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken);
    }
}
