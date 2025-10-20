using ToDo.Application.Interfaces.Repositories;
using ToDo.Domain.Entities;

namespace ToDo.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : GenericRepository<Category, Guid>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}
