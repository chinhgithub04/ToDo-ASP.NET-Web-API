using Microsoft.EntityFrameworkCore;
using ToDo.Application.Interfaces.Services;
using ToDo.Infrastructure.Persistence;

namespace ToDo.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CategoryExistsAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            return await _context.Categories.AnyAsync(c => c.Id == categoryId, cancellationToken);
        }
    }
}
