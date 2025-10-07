using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.Application.Interfaces.Repositories;

namespace ToDo.Application.Interfaces
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        ITodoItemRepository TodoItem { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
