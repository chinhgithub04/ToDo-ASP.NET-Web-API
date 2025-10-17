using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDo.Application.DTOs.Category;
using ToDo.Application.DTOs.Common;
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
        private readonly IMapper _mapper;

        public CategoriesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto<PaginatedResultDto<CategoryDto>>>> GetAllCategories([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var (validPageNumber, validPageSize, validationError) = ValidatePagination(pageNumber, pageSize);
            if (validationError != null)
            {
                return BadRequestResponse<PaginatedResultDto<CategoryDto>>(validationError);
            }

            var userId = GetUserIdFromClaims();

            var paginatedCategories = await _unitOfWork.Category.GetPagedAsync(
                pageNumber,
                pageSize,
                c => c.UserId == userId,
                cancellationToken
            );

            var paginatedResult = _mapper.Map<PaginatedResultDto<CategoryDto>>(paginatedCategories);

            return OkResponse(paginatedResult, "Categories retrieved successfully");
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

            var detailDto = _mapper.Map<DetailCategoryDto>(category);

            return OkResponse(detailDto, "Category retrieved successfully");
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDto<DetailCategoryDto>>> CreateCategory([FromBody] CreateCategoryDto createCategoryDto, [FromServices] IValidator<CreateCategoryDto> validator, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(createCategoryDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                return BadRequestResponse<DetailCategoryDto>("Validation failed", validationResult.Errors.Select(s => s.ErrorMessage).ToList());
            }

            var userId = GetUserIdFromClaims();

            var category = _mapper.Map<Category>(createCategoryDto);
            category.UserId = userId;

            await _unitOfWork.Category.AddAsync(category, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var detailCategoryDto = _mapper.Map<DetailCategoryDto>(category);

            var response = new ResponseDto<DetailCategoryDto>
            {
                Data = detailCategoryDto,
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

        [HttpPatch("{id}")]
        public async Task<ActionResult<ResponseDto<DetailCategoryDto>>> UpdateCategory(Guid id, [FromBody] UpdateCategoryDto updateCategoryDto, [FromServices] IValidator<UpdateCategoryDto> validator, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(updateCategoryDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                return BadRequestResponse<DetailCategoryDto>("Validation failed", validationResult.Errors.Select(s => s.ErrorMessage).ToList());
            }

            var userId = GetUserIdFromClaims();
            var category = await _unitOfWork.Category.GetByIdAsync(id, cancellationToken, c => c.TodoItems);

            if (category == null)
            {
                return NotFoundResponse<DetailCategoryDto>("Category not found");
            }

            if (!await IsCategoryOwnedByUserAsync(category.Id, userId, cancellationToken))
            {
                return ForbiddenResponse<DetailCategoryDto>("You don't have permission to update this category");
            }

            _mapper.Map(updateCategoryDto, category);

            _unitOfWork.Category.Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var detailCategoryDto = _mapper.Map<DetailCategoryDto>(category);

            return OkResponse(detailCategoryDto, "Category updated successfully");
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