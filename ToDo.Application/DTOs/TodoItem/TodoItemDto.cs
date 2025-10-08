namespace ToDo.Application.DTOs.TodoItem
{
    public record TodoItemDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; }
        public bool IsCompleted { get; init; }
    }
}
