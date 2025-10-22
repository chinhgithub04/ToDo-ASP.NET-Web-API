using ToDo.Application.DTOs.Common;
using ToDo.Application.DTOs.TodoItem;
using ToDo.Domain.Entities;

namespace ToDo.Application.Interfaces.Repositories
{
    public interface ITodoItemRepository : IGenericRepository<TodoItem, Guid>
    {
        Task<PaginatedResultDto<TodoItem>> GetTodoItemsWithQueryParametersAsync(string userId, TodoItemQueryParameters queryParameters, CancellationToken cancellationToken);
    }
}