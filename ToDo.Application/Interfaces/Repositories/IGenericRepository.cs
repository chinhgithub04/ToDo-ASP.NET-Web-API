using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ToDo.Application.DTOs.Common;

namespace ToDo.Application.Interfaces.Repositories
{
    public interface IGenericRepository<T, TKey> where T : class
    {
        Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes);
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes);
        Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes);
        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes);

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        Task<PaginatedResultDto<T>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes);

        Task AddAsync(T entity, CancellationToken cancellationToken = default);

        void Update(T entity);
        
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}
