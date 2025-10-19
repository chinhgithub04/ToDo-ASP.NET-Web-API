using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ToDo.Application.DTOs.Common;
using ToDo.Application.DTOs.TodoItem;
using ToDo.Domain.Entities;

namespace ToDo.Application.Interfaces.Repositories
{
    public interface ITodoItemRepository : IGenericRepository<TodoItem, Guid>
    {
        Task<IReadOnlyList<TodoItem>> GetTodosByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default, params Expression<Func<TodoItem, object>>[] includes);
        Task<IReadOnlyList<TodoItem>> GetTodosByUserAsync(string userId, CancellationToken cancellationToken = default, params Expression<Func<TodoItem, object>>[] includes);
        Task<PaginatedResultDto<TodoItem>> GetTodoItemsAsync(string userId, TodoItemQueryParameters queryParameters, CancellationToken cancellationToken);
    }
}