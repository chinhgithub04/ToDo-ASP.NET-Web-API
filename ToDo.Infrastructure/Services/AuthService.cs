using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ToDo.Application.DTOs.Auth;
using ToDo.Application.Interfaces.Services;
using ToDo.Application.Settings;
using ToDo.Domain.Constants;
using ToDo.Domain.Entities;

namespace ToDo.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<JwtSettings> jwtSettings,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Login attempt for email: {Email}", loginDto.Email);

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !user.IsActive)
            {
                _logger.LogWarning("Login failed: User not found or inactive for email: {Email}", loginDto.Email);
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Login failed: Invalid password for email: {Email}", loginDto.Email);
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            _logger.LogInformation("Login successful for user: {UserId}", user.Id);
            var roles = await _userManager.GetRolesAsync(user);
            return await GenerateTokenResponseAsync(user, roles);
        }

        public async Task<LoginResponseDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Registration attempt for email: {Email}", registerDto.Email);

            var user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(" ", result.Errors.Select(e => e.Description));
                _logger.LogError("Registration failed for email {Email}: {Errors}", registerDto.Email, errors);
                throw new InvalidOperationException($"Failed to register user: {errors}");
            }

            await _userManager.AddToRoleAsync(user, UserRoles.User);

            _logger.LogInformation("Registration successful for user: {UserId}", user.Id);
            var roles = await _userManager.GetRolesAsync(user);
            return await GenerateTokenResponseAsync(user, roles);
        }

        private async Task<LoginResponseDto> GenerateTokenResponseAsync(ApplicationUser user, IList<string> roles)
        {
            var tokenExpiryTime = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes);
            
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("first_name", user.FirstName),
                new Claim("last_name", user.LastName)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: tokenExpiryTime,
                signingCredentials: credentials
            );

            return new LoginResponseDto
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = tokenExpiryTime,
                UserId = user.Id,
                Email = user.Email,
                Roles = roles
            };
        }
    }
}