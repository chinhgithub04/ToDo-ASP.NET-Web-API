namespace ToDo.Application.DTOs.Category
{
    public record CreateCategoryDto
    {
        public string Name { get; init; }
        public string Color { get; init; }
    }
}