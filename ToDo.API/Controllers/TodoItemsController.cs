using AutoMapper;
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
    public class TodoItemsController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TodoItemsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto<PaginatedResultDto<TodoItemDto>>>> GetAllTodoItems([FromQuery] TodoItemQueryParameters queryParameters, [FromServices] IValidator<TodoItemQueryParameters> validator, CancellationToken cancellationToken = default)
        {
            var validationResult = await validator.ValidateAsync(queryParameters, cancellationToken);
            if (!validationResult.IsValid)
            {
                return BadRequestResponse<PaginatedResultDto<TodoItemDto>>("Validation failed", validationResult.Errors.Select(s => s.ErrorMessage).ToList());
            }

            var userId = GetUserIdFromClaims();
            var paginatedTodoItems = await _unitOfWork.TodoItem.GetTodoItemsAsync(userId, queryParameters, cancellationToken);
            var paginatedResult = _mapper.Map<PaginatedResultDto<TodoItemDto>>(paginatedTodoItems);

            return OkResponse(paginatedResult, "Todo items retrieved successfully");
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

            var detailDto = _mapper.Map<DetailTodoItemDto>(todoItem);

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

            var todoItem = _mapper.Map<TodoItem>(createTodoItemDto);
            todoItem.UserId = userId;

            await _unitOfWork.TodoItem.AddAsync(todoItem, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (todoItem.CategoryId.HasValue)
            {
                todoItem = await _unitOfWork.TodoItem.GetByIdAsync(todoItem.Id, cancellationToken, t => t.Category);
            }

            var detailTodoItemDto = _mapper.Map<DetailTodoItemDto>(todoItem);

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

            _mapper.Map(updateTodoItemDto, todoItem);

            _unitOfWork.TodoItem.Update(todoItem);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var updatedTodoItem = await _unitOfWork.TodoItem.GetByIdAsync(todoItem.Id, cancellationToken, t => t.Category);

            var detailTodoItemDto = _mapper.Map<DetailTodoItemDto>(updatedTodoItem);

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