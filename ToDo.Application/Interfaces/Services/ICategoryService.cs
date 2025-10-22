using ToDo.Application.DTOs.Category;
using ToDo.Application.DTOs.Common;

namespace ToDo.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<PaginatedResultDto<CategoryDto>> GetAllCategoriesAsync(string userId, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<DetailCategoryDto> GetCategoryByIdAsync(Guid id, string userId, CancellationToken cancellationToken);
        Task<DetailCategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto, string userId, CancellationToken cancellationToken);
        Task<DetailCategoryDto> UpdateCategoryAsync (Guid id, UpdateCategoryDto updateCategoryDto, string userId, CancellationToken cancellationToken);
        Task DeleteCategoryAsync (Guid id, string userId, CancellationToken cancellationToken);
    }
}
