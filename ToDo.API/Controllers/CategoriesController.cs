using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDo.Application.DTOs.Category;
using ToDo.Application.DTOs.Common;
using ToDo.Application.Interfaces.Services;

namespace ToDo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : BaseApiController
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto<PaginatedResultDto<CategoryDto>>>> GetAllCategories([FromQuery] PaginationQuery query, CancellationToken cancellationToken = default)
        {
            var userId = GetUserIdFromClaims();
            var paginatedResult = await _categoryService.GetAllCategoriesAsync(userId, query.PageNumber, query.PageSize, cancellationToken);

            return OkResponse(paginatedResult, "Categories retrieved successfully");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto<DetailCategoryDto>>> GetCategoryById(Guid id, CancellationToken cancellationToken = default)
        {
            var userId = GetUserIdFromClaims();
            var detailCategoryDto = await _categoryService.GetCategoryByIdAsync(id, userId, cancellationToken);

            return OkResponse(detailCategoryDto, "Category retrieved successfully");
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDto<DetailCategoryDto>>> CreateCategory([FromBody] CreateCategoryDto createCategoryDto, CancellationToken cancellationToken = default)
        {
            var userId = GetUserIdFromClaims();

            var detailCategoryDto = await _categoryService.CreateCategoryAsync(createCategoryDto, userId, cancellationToken);

            var response = new ResponseDto<DetailCategoryDto>
            {
                Data = detailCategoryDto,
                IsSuccess = true,
                Message = "Category created successfully",
                StatusCode = 201
            };

            return CreatedAtAction(
                nameof(GetCategoryById),
                new { id = detailCategoryDto.Id },
                response
            );
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<ResponseDto<DetailCategoryDto>>> UpdateCategory(Guid id, [FromBody] UpdateCategoryDto updateCategoryDto, CancellationToken cancellationToken = default)
        {
            var userId = GetUserIdFromClaims();

            var detailCategoryDto = await _categoryService.UpdateCategoryAsync(id, updateCategoryDto, userId, cancellationToken);

            return OkResponse(detailCategoryDto, "Category updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken = default)
        {
            var userId = GetUserIdFromClaims();

            await _categoryService.DeleteCategoryAsync(id, userId, cancellationToken);

            return NoContent();
        }
    }
}