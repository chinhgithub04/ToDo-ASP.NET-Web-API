using ToDo.Domain.Enums;

namespace ToDo.Application.DTOs.TodoItem
{
    public record TodoItemQueryParameters
    {
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;

        // Filtering
        public bool? IsCompleted { get; init; }
        public Guid? CategoryId { get; init; }
        public string? SearchTerm { get; init; }

        // Sorting
        public SortBy? SortBy { get; init; }
        public SortOrder? SortOrder { get; init; }
    }
}
