using ToDo.Domain.Enums;

namespace ToDo.Application.DTOs.TodoItem
{
    public record TodoItemListDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; }
        public string? DescriptionPreview { get; init; }
        public bool IsCompleted { get; init; }
        public DateTimeOffset? DueDate { get; init; }
        public Priority Priority { get; init; }
        public Guid? CategoryId { get; init; }
        public string? CategoryName { get; init; }
        public string? CategoryColor { get; init; }
    }
}
