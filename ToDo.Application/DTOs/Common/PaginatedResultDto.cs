namespace ToDo.Application.DTOs.Common
{
    public record PaginatedResultDto<T>
    {
        public List<T> Items { get; init; }
        public int TotalCount { get; init; }
        public int PageSize { get; init; }
        public int CurrentPage { get; init; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
