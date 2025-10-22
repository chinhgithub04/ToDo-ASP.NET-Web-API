using ToDo.Domain.Entities;

namespace ToDo.Application.Interfaces.Repositories
{
    public interface ICategoryRepository : IGenericRepository<Category, Guid>
    {
        Task<bool> IsCategoryExistsAndOwnByUserAsync(Guid categoryId, string userId, CancellationToken cancellationToken);
        Task<bool> IsCategoryNameExistAsync(string categoryName, string userId, CancellationToken cancellationToken);
    }
}
