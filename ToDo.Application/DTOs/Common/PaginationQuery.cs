namespace ToDo.Application.DTOs.Common
{
    public record PaginationQuery
    {
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }
}
