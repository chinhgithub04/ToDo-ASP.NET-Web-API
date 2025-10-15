using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDo.Application.DTOs.Common;
using ToDo.Application.DTOs.TodoItem;
using ToDo.Application.Interfaces;
using ToDo.Domain.Entities;

namespace ToDo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TodoItemsController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public TodoItemsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto<IEnumerable<TodoItemDto>>>> GetAllTodoItems(CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromClaims();
            var todoItems = await _unitOfWork.TodoItem.FindAsync(t => t.UserId == userId, cancellationToken);

            var todoItemDtos = todoItems.Select(t => new TodoItemDto
            {
                Id = t.Id,
                Title = t.Title,
                IsCompleted = t.IsCompleted
            });

            return OkResponse(todoItemDtos, "Todo items retrieved successfully");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto<DetailTodoItemDto>>> GetTodoItemById(Guid id, CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromClaims();
            var todoItem = await _unitOfWork.TodoItem.GetByIdAsync(id, cancellationToken, t => t.Category);

            if (todoItem == null)
            {
                return NotFoundResponse<DetailTodoItemDto>("Todo item not found");
            }

            if (!await IsTodoItemOwnedByUserAsync(todoItem.Id, userId, cancellationToken))
            {
                return ForbiddenResponse<DetailTodoItemDto>("You don't have permission to access this todo item");
            }

            var detailDto = new DetailTodoItemDto
            {
                Id = todoItem.Id,
                Title = todoItem.Title,
                Description = todoItem.Description,
                IsCompleted = todoItem.IsCompleted,
                CategoryId = todoItem.CategoryId,
                CategoryName = todoItem.Category?.Name,
                CreatedAt = todoItem.CreatedAt,
                UpdatedAt = todoItem.UpdatedAt,
            };

            return OkResponse(detailDto, "Todo item retrieved successfully");
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDto<DetailTodoItemDto>>> CreateTodoItem([FromBody] CreateTodoItemDto createTodoItemDto, [FromServices] IValidator<CreateTodoItemDto> validator, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(createTodoItemDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                return BadRequestResponse<DetailTodoItemDto>("Validation failed", validationResult.Errors.Select(s => s.ErrorMessage).ToList());
            }

            var userId = GetUserIdFromClaims();

            var todoItem = new TodoItem
            {
                Title = createTodoItemDto.Title,
                Description = createTodoItemDto.Description,
                CategoryId = createTodoItemDto.CategoryId,
                UserId = userId
            };

            await _unitOfWork.TodoItem.AddAsync(todoItem, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (todoItem.CategoryId.HasValue)
            {
                todoItem = await _unitOfWork.TodoItem.GetByIdAsync(todoItem.Id, cancellationToken, t => t.Category);
            }

            var detailTodoItemDto = new DetailTodoItemDto
            {
                Id = todoItem.Id,
                Title = todoItem.Title,
                Description = todoItem.Description,
                IsCompleted = todoItem.IsCompleted,
                CategoryId = todoItem.CategoryId,
                CategoryName = todoItem.Category?.Name,
                CreatedAt = todoItem.CreatedAt,
                UpdatedAt = todoItem.UpdatedAt
            };

            var response = new ResponseDto<DetailTodoItemDto>
            {
                Data = detailTodoItemDto,
                IsSuccess = true,
                Message = "Todo item created successfully",
                StatusCode = 201
            };

            return CreatedAtAction(nameof(GetTodoItemById), new { id = todoItem.Id }, response);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<ResponseDto<DetailTodoItemDto>>> UpdateTodoItem(Guid id, [FromBody] UpdateTodoItemDto updateTodoItemDto, [FromServices] IValidator<UpdateTodoItemDto> validator, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(updateTodoItemDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                return BadRequestResponse<DetailTodoItemDto>("Validation failed", validationResult.Errors.Select(s => s.ErrorMessage).ToList());
            }

            var userId = GetUserIdFromClaims();
            var todoItem = await _unitOfWork.TodoItem.GetByIdAsync(id, cancellationToken, t => t.Category);

            if (todoItem == null)
            {
                return NotFoundResponse<DetailTodoItemDto>("Todo item not found");
            }

            if (!await IsTodoItemOwnedByUserAsync(todoItem.Id, userId, cancellationToken))
            {
                return ForbiddenResponse<DetailTodoItemDto>("You don't have permission to update this todo item");
            }

            if (!string.IsNullOrWhiteSpace(updateTodoItemDto.Title))
            {
                todoItem.Title = updateTodoItemDto.Title;
            }

            if (!string.IsNullOrWhiteSpace(updateTodoItemDto.Description))
            {
                todoItem.Description = updateTodoItemDto.Description;
            }

            if (updateTodoItemDto.IsCompleted.HasValue)
            {
                todoItem.IsCompleted = updateTodoItemDto.IsCompleted.Value;
            }

            if (updateTodoItemDto.CategoryId.HasValue)
            {
                todoItem.CategoryId = updateTodoItemDto.CategoryId.Value;
            }


            _unitOfWork.TodoItem.Update(todoItem);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var updatedTodoItem = await _unitOfWork.TodoItem.GetByIdAsync(todoItem.Id, cancellationToken, t => t.Category);

            var detailTodoItemDto = new DetailTodoItemDto
            {
                Id = updatedTodoItem.Id,
                Title = updatedTodoItem.Title,
                Description = updatedTodoItem.Description,
                IsCompleted = updatedTodoItem.IsCompleted,
                CategoryId = updatedTodoItem.CategoryId,
                CategoryName = updatedTodoItem.Category?.Name,
                CreatedAt = updatedTodoItem.CreatedAt,
                UpdatedAt = updatedTodoItem.UpdatedAt
            };
            return OkResponse(detailTodoItemDto, "Todo item updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseDto<object>>> DeleteTodoItem(Guid id, CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromClaims();
            var todoItem = await _unitOfWork.TodoItem.GetByIdAsync(id, cancellationToken);

            if (todoItem == null)
            {
                return NotFoundResponse<object>("Todo item not found");
            }

            if (!await IsTodoItemOwnedByUserAsync(todoItem.Id, userId, cancellationToken))
            {
                return ForbiddenResponse<object>("You don't have permission to delete this todo item");
            }

            _unitOfWork.TodoItem.Remove(todoItem);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        private async Task<bool> IsTodoItemOwnedByUserAsync(Guid todoItemId, string userId, CancellationToken cancellationToken)
        {
            var todoItem = await _unitOfWork.TodoItem.GetFirstOrDefaultAsync(t => t.Id == todoItemId && t.UserId == userId, cancellationToken);

            return todoItem != null;
        }
    }
}
