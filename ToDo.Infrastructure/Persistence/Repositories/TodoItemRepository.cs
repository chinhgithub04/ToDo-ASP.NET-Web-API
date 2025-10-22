using Microsoft.EntityFrameworkCore;
using ToDo.Application.DTOs.Common;
using ToDo.Application.DTOs.TodoItem;
using ToDo.Application.Interfaces.Repositories;
using ToDo.Domain.Entities;
using ToDo.Domain.Enums;

namespace ToDo.Infrastructure.Persistence.Repositories
{
    public class TodoItemRepository : GenericRepository<TodoItem, Guid>, ITodoItemRepository
    {
        public TodoItemRepository(ApplicationDbContext db) : base(db)
        {
        }

        public async Task<PaginatedResultDto<TodoItem>> GetTodoItemsWithQueryParametersAsync(string userId, TodoItemQueryParameters queryParameters, CancellationToken cancellationToken)
        {
            IQueryable<TodoItem> query = _dbSet.AsNoTracking().Where(t => t.UserId == userId).Include(t => t.Category);

            // Filter
            if (queryParameters.IsCompleted.HasValue)
            {
                query = query.Where(t => t.IsCompleted == queryParameters.IsCompleted.Value);
            }

            if (queryParameters.CategoryId.HasValue)
            {
                query = query.Where(t => t.CategoryId == queryParameters.CategoryId.Value);
            }

            if (!string.IsNullOrEmpty(queryParameters.SearchTerm))
            {
                query = query.Where(t => EF.Functions.Like(t.Title, $"%{queryParameters.SearchTerm}%"));
            }

            // Sort
            bool isDescending = queryParameters.SortOrder == SortOrder.Desc;

            query = queryParameters.SortBy switch
            {
                SortBy.UpdatedAt => isDescending
                ? query.OrderBy(t => t.UpdatedAt == null).ThenByDescending(t => t.UpdatedAt)
                : query.OrderBy(t => t.UpdatedAt == null).ThenBy(t => t.UpdatedAt),

                SortBy.Title => isDescending
                ? query.OrderByDescending(t => t.Title)
                : query.OrderBy(t => t.Title),

                SortBy.IsCompleted => isDescending
                ? query.OrderByDescending(t => t.IsCompleted)
                : query.OrderBy(t => t.IsCompleted),

                SortBy.CreatedAt or _ => isDescending
                ? query.OrderByDescending(t => t.CreatedAt)
                : query.OrderBy(t => t.CreatedAt),
            };

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedResultDto<TodoItem>
            {
                Items = items,
                TotalCount = totalCount,
                PageSize = queryParameters.PageSize,
                PageNumber = queryParameters.PageNumber
            };
        }
    }
}