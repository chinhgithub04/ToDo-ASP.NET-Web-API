using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.Application.Interfaces;
using ToDo.Application.Interfaces.Repositories;
using ToDo.Infrastructure.Persistence.Repositories;

namespace ToDo.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository Category { get; private set; }
        public ITodoItemRepository TodoItem { get; private set; }

        private readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context, ICategoryRepository categoryRepository, ITodoItemRepository todoItemRepository)
        {
            _context = context;
            Category = categoryRepository;
            TodoItem = todoItemRepository;
        }


        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
