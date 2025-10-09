namespace ToDo.Application.DTOs.Category
{
    public record UpdateCategoryDto
    {
        public string? Name { get; init; }
        public string? Color { get; init; }
    }
}
