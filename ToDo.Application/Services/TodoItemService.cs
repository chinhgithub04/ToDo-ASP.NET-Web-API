using AutoMapper;
using Microsoft.Extensions.Logging;
using ToDo.Application.DTOs.Common;
using ToDo.Application.DTOs.TodoItem;
using ToDo.Application.Interfaces;
using ToDo.Application.Interfaces.Services;
using ToDo.Domain.Entities;
using ToDo.Domain.Exceptions;

namespace ToDo.Application.Services
{
    public class TodoItemService : ITodoItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TodoItemService> _logger;

        public TodoItemService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TodoItemService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResultDto<TodoItemDto>> GetAllTodoItemsAsync(TodoItemQueryParameters queryParameters, string userId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving TodoItems for User {UserId} - PageNumber: {PageNumber}, PageSize: {PageSize}", userId, queryParameters.PageNumber, queryParameters.PageSize);

            var paginatedTodoItems = await _unitOfWork.TodoItem.GetTodoItemsWithQueryParametersAsync(userId, queryParameters, cancellationToken);

            var paginatedResult = _mapper.Map<PaginatedResultDto<TodoItemDto>>(paginatedTodoItems);
            _logger.LogInformation("Successfully retrieved {ItemCount} TodoItems for User {UserId}", paginatedResult.Items.Count, userId);

            return paginatedResult;
        }

        public async Task<DetailTodoItemDto> GetTodoItemByIdAsync(Guid id, string userId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving TodoItem {TodoItemId}", id);

            var todoItem = await _unitOfWork.TodoItem.GetByIdAsync(id, cancellationToken, t => t.Category);

            if (todoItem == null)
            {
                _logger.LogWarning("TodoItem {TodoItemId} not found", id);
                throw new NotFoundException($"Todo Item with ID {id} not found.");
            }

            if (todoItem.UserId != userId)
            {
                _logger.LogWarning("User {UserId} attempted to access forbidden TodoItem {TodoItemId}", userId, id);
                throw new ForbiddenAccessException("You don't have permission to access this Todo Item.");
            }

            _logger.LogInformation("Successfully retrieved TodoItem {TodoItemID}", id);

            return _mapper.Map<DetailTodoItemDto>(todoItem);
        }

        public async Task<DetailTodoItemDto> CreateTodoItemAsync(CreateTodoItemDto createTodoItemDto, string userId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to create TodoItem {Title} for User {UserId}", createTodoItemDto.Title, userId);
            
            if (createTodoItemDto.CategoryId.HasValue)
            {
                if (!await _unitOfWork.Category.IsCategoryExistsAndOwnByUserAsync(createTodoItemDto.CategoryId.Value, userId, cancellationToken))
                {
                    _logger.LogWarning("Create TodoItem failed: CategoryId {CategoryId} not found or not owned by user {UserId}.", createTodoItemDto.CategoryId.Value, userId);
                    throw new InvalidOperationException($"Category with ID {createTodoItemDto.CategoryId.Value} not found or you don't have permission to use it.");
                }
            }

            var todoItem = _mapper.Map<TodoItem>(createTodoItemDto);
            todoItem.UserId = userId;

            await _unitOfWork.TodoItem.AddAsync(todoItem, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("TodoItem {TodoItemId} created successfully.", todoItem.Id);

            if (todoItem.CategoryId.HasValue)
            {
                todoItem = await _unitOfWork.TodoItem.GetByIdAsync(todoItem.Id, cancellationToken, t => t.Category);
            }

            return _mapper.Map<DetailTodoItemDto>(todoItem);
        }

        public async Task<DetailTodoItemDto> UpdateTodoItemAsync(Guid id, UpdateTodoItemDto updateTodoItemDto, string userId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to update TodoItem {Title} for User {UserId}", updateTodoItemDto.Title, userId);

            var todoItem = await _unitOfWork.TodoItem.GetByIdAsync(id, cancellationToken, t => t.Category);

            if (todoItem == null)
            {
                _logger.LogWarning("Update failed: TodoItem {TodoItemId} not found", id);
                throw new NotFoundException($"TodoItem with ID {id} not found.");
            }

            if (todoItem.UserId != userId)
            {
                _logger.LogWarning("Update forbidden: User {UserId} attempted to update TodoItem {TodoItemId}", userId, id);
                throw new ForbiddenAccessException("You don't have permission to update this TodoItem.");
            }

            if (updateTodoItemDto.CategoryId.HasValue && updateTodoItemDto.CategoryId != todoItem.CategoryId)
            {
                if (!await _unitOfWork.Category.IsCategoryExistsAndOwnByUserAsync(updateTodoItemDto.CategoryId.Value, userId, cancellationToken))
                {
                    _logger.LogWarning("Update TodoItem failed: New CategoryId {CategoryId} not found or not owned by user {UserId}.", updateTodoItemDto.CategoryId.Value, userId);
                    throw new InvalidOperationException($"Category with ID {updateTodoItemDto.CategoryId.Value} not found or you don't have permission to use it.");
                }
            }

            _mapper.Map(updateTodoItemDto, todoItem);

            _unitOfWork.TodoItem.Update(todoItem);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("TodoItem {TodoItemId} updated successfully", todoItem.Id);
            return _mapper.Map<DetailTodoItemDto>(todoItem);
        }
        public async Task DeleteTodoItemAsync(Guid id, string userId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to delete TodoItem {TodoItemId} for user {UserId}", id, userId);

            var todoItem = await _unitOfWork.TodoItem.GetByIdAsync(id, cancellationToken);

            if (todoItem == null)
            {
                _logger.LogWarning("Delete ignored: TodoItem {TodoItemId} not found", id);
                throw new NotFoundException($"TodoItem with ID {id} not found.");
            }

            if (todoItem.UserId != userId)
            {
                _logger.LogWarning("Delete forbidden: User {UserId} attempted to delete TodoItem {TodoItemId}", userId, id);
                throw new ForbiddenAccessException("You don't have permission to delete this TodoItem.");
            }

            _unitOfWork.TodoItem.Remove(todoItem);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("TodoItem {TodoItemId} deleted successfully", id);
        }
    }
}
