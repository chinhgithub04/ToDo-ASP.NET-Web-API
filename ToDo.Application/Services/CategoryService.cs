using AutoMapper;
using Microsoft.Extensions.Logging;
using ToDo.Application.DTOs.Category;
using ToDo.Application.DTOs.Common;
using ToDo.Application.Interfaces;
using ToDo.Application.Interfaces.Services;
using ToDo.Domain.Entities;
using ToDo.Domain.Exceptions;

namespace ToDo.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResultDto<CategoryDto>> GetAllCategoriesAsync(string userId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving categories for User {UserId} - PageNumber: {PageNumber}, PageSize: {PageSize}", userId, pageNumber, pageSize);

            var paginatedCategories = await _unitOfWork.Category.GetPagedAsync(
                pageNumber,
                pageSize,
                c => c.UserId == userId,
                cancellationToken
            );

            var paginatedResult = _mapper.Map<PaginatedResultDto<CategoryDto>>(paginatedCategories);
            _logger.LogInformation("Successfully retrieved {ItemCount} categories for User {UserId}", paginatedResult.Items.Count, userId);

            return paginatedResult;
        }

        public async Task<DetailCategoryDto> GetCategoryByIdAsync(Guid id, string userId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving Category {CategoryId}", id);

            var category = await _unitOfWork.Category.GetByIdAsync(id, cancellationToken, c => c.TodoItems);

            if (category == null)
            {
                _logger.LogWarning("Category {CategoryId} not found", id);
                throw new NotFoundException($"Category with ID {id} not found.");
            }

            if (category.UserId != userId)
            {
                _logger.LogWarning("User {UserId} attempted to access forbidden category {CategoryId}", userId, id);
                throw new ForbiddenAccessException("You don't have permission to access this category.");
            }

            _logger.LogInformation("Successfully retrieved category {CategoryId}", id);
            return _mapper.Map<DetailCategoryDto>(category);
        }

        public async Task<DetailCategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto, string userId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to create category {Name} for user {UserId}", createCategoryDto.Name, userId);

            if (await _unitOfWork.Category.IsCategoryNameExistAsync(createCategoryDto.Name, userId, cancellationToken))
            {
                _logger.LogWarning("Category creation failed: Name {Name} already exists for user {UserId}", createCategoryDto.Name, userId);
                throw new InvalidOperationException("A category with this name already exists.");
            }

            var category = _mapper.Map<Category>(createCategoryDto);
            category.UserId = userId;

            await _unitOfWork.Category.AddAsync(category, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Category {Name} created successfully with Id {CategoryId}", category.Name, category.Id);
            return _mapper.Map<DetailCategoryDto>(category);
        }

        public async Task<DetailCategoryDto> UpdateCategoryAsync(Guid id, UpdateCategoryDto updateCategoryDto, string userId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to update category {CategoryId} for user {UserId}", id, userId);

            var category = await _unitOfWork.Category.GetByIdAsync(id, cancellationToken, c => c.TodoItems);

            if (category == null)
            {
                _logger.LogWarning("Update failed: Category {CategoryId} not found", id);
                throw new NotFoundException($"Category with ID {id} not found.");
            }

            if (category.UserId != userId)
            {
                _logger.LogWarning("Update forbidden: User {UserId} attempted to update category {CategoryId}", userId, id);
                throw new ForbiddenAccessException("You don't have permission to update this category.");
            }

            if (!string.IsNullOrWhiteSpace(updateCategoryDto.Name) && updateCategoryDto.Name != category.Name)
            {
                if (await _unitOfWork.Category.IsCategoryNameExistAsync(updateCategoryDto.Name, userId, cancellationToken))
                {
                    _logger.LogWarning("Update failed: Name {Name} already exists for user {UserId}", updateCategoryDto.Name, userId);
                    throw new InvalidOperationException("A category with this name already exists.");
                }
            }

            _mapper.Map(updateCategoryDto, category);

            _unitOfWork.Category.Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Category {CategoryId} updated successfully", category.Id);
            return _mapper.Map<DetailCategoryDto>(category);
        }

        public async Task DeleteCategoryAsync(Guid id, string userId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to delete category {CategoryId} for user {UserId}", id, userId);

            var category = await _unitOfWork.Category.GetByIdAsync(id, cancellationToken);

            if (category == null)
            {
                _logger.LogWarning("Delete ignored: Category {CategoryId} not found", id);
                throw new NotFoundException($"Category with ID {id} not found.");
            }

            if (category.UserId != userId)
            {
                _logger.LogWarning("Delete forbidden: User {UserId} attempted to delete category {CategoryId}", userId, id);
                throw new ForbiddenAccessException("You don't have permission to delete this category.");
            }

            _unitOfWork.Category.Remove(category);  
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Category {CategoryId} deleted successfully", id);
        }
    }
}
