using ToDo.Domain.Enums;

namespace ToDo.Application.DTOs.TodoItem
{
    public record CreateTodoItemDto
    {
        public string Title { get; init; }
        public string? Description { get; init; }
        public Guid? CategoryId { get; init; }
        public DateTimeOffset? DueDate { get; init; }
        public Priority Priority { get; init; } = Priority.Low;
    }
}
