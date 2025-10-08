namespace ToDo.Application.DTOs.Auth
{
    public record LoginDto
    {
        public string Email { get; init; }
        public string Password { get; init; }
    }
}
