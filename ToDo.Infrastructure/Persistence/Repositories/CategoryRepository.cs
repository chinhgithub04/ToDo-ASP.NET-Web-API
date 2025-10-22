using ToDo.Application.Interfaces.Repositories;
using ToDo.Domain.Entities;

namespace ToDo.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : GenericRepository<Category, Guid>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
        }

        public async Task<bool> IsCategoryExistsAndOwnByUserAsync(Guid categoryId, string userId, CancellationToken cancellationToken = default)
        {
            return await AnyAsync(c => c.Id == categoryId && c.UserId == userId, cancellationToken);
        }

        public async Task<bool> IsCategoryNameExistAsync(string categoryName, string userId, CancellationToken cancellationToken = default)
        {
            return await AnyAsync(c => c.Name.Equals(categoryName) && c.UserId == userId, cancellationToken);
        }
    }
}
