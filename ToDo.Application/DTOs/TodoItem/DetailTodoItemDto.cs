namespace ToDo.Application.DTOs.TodoItem
{
    public record DetailTodoItemDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; }
        public string? Description { get; init; }
        public bool IsCompleted { get; init; }
        public Guid? CategoryId { get; init; }
        public string? CategoryName { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public DateTimeOffset? UpdatedAt { get; init; }
    }
}
