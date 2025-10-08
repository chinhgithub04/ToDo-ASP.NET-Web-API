using ToDo.Application.DTOs.TodoItem;

namespace ToDo.Application.DTOs.Category
{
    public record DetailCategoryDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Color { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public DateTimeOffset? UpdatedAt { get; init; }
        public List<TodoItemDto> TodoItems { get; init; }
    }
}
