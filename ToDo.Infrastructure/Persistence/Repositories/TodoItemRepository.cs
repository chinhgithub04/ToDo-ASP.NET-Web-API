using System.Linq.Expressions;
using ToDo.Application.Interfaces.Repositories;
using ToDo.Domain.Entities;

namespace ToDo.Infrastructure.Persistence.Repositories
{
    public class TodoItemRepository : GenericRepository<TodoItem, Guid>, ITodoItemRepository
    {
        public TodoItemRepository(ApplicationDbContext db) : base(db)
        {
        }

        public async Task<IReadOnlyList<TodoItem>> GetTodosByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default, params Expression<Func<TodoItem, object>>[] includes)
        {
            return await FindAsync(t => t.CategoryId == categoryId, cancellationToken, includes);
        }

        public async Task<IReadOnlyList<TodoItem>> GetTodosByUserAsync(string userId, CancellationToken cancellationToken = default, params Expression<Func<TodoItem, object>>[] includes)
        {
            return await FindAsync(t => t.UserId == userId, cancellationToken, includes);
        }
    }
}