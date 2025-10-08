namespace ToDo.Application.DTOs.Category
{
    public record CategoryDto
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }
        public string? Color { get; init; }
    }
}
