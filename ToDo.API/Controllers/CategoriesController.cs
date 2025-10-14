using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDo.Application.DTOs.Category;
using ToDo.Application.DTOs.Common;
using ToDo.Application.DTOs.TodoItem;
using ToDo.Application.Interfaces;
using ToDo.Domain.Entities;

namespace ToDo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto<IEnumerable<CategoryDto>>>> GetAllCategories(CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromClaims();
            var categories = await _unitOfWork.Category.FindAsync(
                c => c.UserId == userId,
                cancellationToken
            );

            var categoryDtos = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Color = c.Color
            });

            return OkResponse(categoryDtos, "Categories retrieved successfully");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto<DetailCategoryDto>>> GetCategoryById(Guid id, CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromClaims();
            var category = await _unitOfWork.Category.GetByIdAsync(
                id,
                cancellationToken,
                c => c.TodoItems
            );

            if (category == null)
            {
                return NotFoundResponse<DetailCategoryDto>("Category not found");
            }

            if (!await IsCategoryOwnedByUserAsync(category.Id, userId, cancellationToken))
            {
                return ForbiddenResponse<DetailCategoryDto>("You don't have permission to access this category");
            }

            var detailDto = new DetailCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Color = category.Color,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt,
                TodoItems = category.TodoItems.Select(t => new TodoItemDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    IsCompleted = t.IsCompleted,
                }).ToList()
            };

            return OkResponse(detailDto, "Category retrieved successfully");
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDto<CategoryDto>>> CreateCategory([FromBody] CreateCategoryDto createCategoryDto, [FromServices] IValidator<CreateCategoryDto> validator, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(createCategoryDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                return BadRequestResponse<CategoryDto>("Validation failed", validationResult.Errors.Select(s => s.ErrorMessage).ToList());
            }

            var userId = GetUserIdFromClaims();

            var category = new Category
            {
                Name = createCategoryDto.Name,
                Color = createCategoryDto.Color,
                UserId = userId
            };

            await _unitOfWork.Category.AddAsync(category, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Color = category.Color
            };

            var response = new ResponseDto<CategoryDto>
            {
                Data = categoryDto,
                IsSuccess = true,
                Message = "Category created successfully",
                StatusCode = 201
            };

            return CreatedAtAction(
                nameof(GetCategoryById),
                new { id = category.Id },
                response
            );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseDto<CategoryDto>>> UpdateCategory(Guid id, [FromBody] UpdateCategoryDto updateCategoryDto, [FromServices] IValidator<UpdateCategoryDto> validator, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(updateCategoryDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                return BadRequestResponse<CategoryDto>("Validation failed", validationResult.Errors.Select(s => s.ErrorMessage).ToList());
            }

            var userId = GetUserIdFromClaims();
            var category = await _unitOfWork.Category.GetByIdAsync(id, cancellationToken);

            if (category == null)
            {
                return NotFoundResponse<CategoryDto>("Category not found");
            }

            if (!await IsCategoryOwnedByUserAsync(category.Id, userId, cancellationToken))
            {
                return ForbiddenResponse<CategoryDto>("You don't have permission to update this category");
            }

            if (!string.IsNullOrWhiteSpace(updateCategoryDto.Name))
            {
                category.Name = updateCategoryDto.Name;
            }

            if (!string.IsNullOrWhiteSpace(updateCategoryDto.Color))
            {
                category.Color = updateCategoryDto.Color;
            }

            _unitOfWork.Category.Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Color = category.Color
            };

            return OkResponse(categoryDto, "Category updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseDto<object>>> DeleteCategory(Guid id, CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromClaims();
            var category = await _unitOfWork.Category.GetByIdAsync(id, cancellationToken);

            if (category == null)
            {
                return NotFoundResponse<object>("Category not found");
            }

            if (!await IsCategoryOwnedByUserAsync(category.Id, userId, cancellationToken))
            {
                return ForbiddenResponse<object>("You don't have permission to delete this category");
            }

            _unitOfWork.Category.Remove(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        private async Task<bool> IsCategoryOwnedByUserAsync(Guid categoryId, string userId, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Category.GetFirstOrDefaultAsync(
                c => c.Id == categoryId && c.UserId == userId,
                cancellationToken
            );

            return category != null;
        }
    }
}