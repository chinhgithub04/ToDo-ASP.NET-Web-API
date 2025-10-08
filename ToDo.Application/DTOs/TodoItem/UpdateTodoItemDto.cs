namespace ToDo.Application.DTOs.TodoItem
{
    public record UpdateTodoItemDto
    {
        public string? Title { get; init; }
        public string? Description { get; init; }
        public bool? IsCompleted { get; init; }
        public Guid? CategoryId { get; init; }
    }
}
