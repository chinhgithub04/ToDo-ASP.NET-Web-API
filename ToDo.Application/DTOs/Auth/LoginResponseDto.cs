namespace ToDo.Application.DTOs.Auth
{
    public record LoginResponseDto
    {
        public string AccessToken { get; init; }
        public DateTimeOffset ExpiresAt { get; init; }
        public string UserId { get; init; }
        public string Email { get; init; }
        public IList<string> Roles { get; init; }
    }
}