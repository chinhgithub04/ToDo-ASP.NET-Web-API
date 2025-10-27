using ToDo.Application.DTOs.Common;
using ToDo.Domain.Enums;

namespace ToDo.Application.DTOs.TodoItem
{
    public record TodoItemQueryParameters : PaginationQuery
    {
        // Filtering
        public bool? IsCompleted { get; init; }
        public Guid? CategoryId { get; init; }
        public string? SearchTerm { get; init; }
        public Priority? Priority { get; init; }
        public bool? IsOverDue { get; init; }

        // Sorting
        public SortBy? SortBy { get; init; }
        public SortOrder? SortOrder { get; init; }
    }
}
