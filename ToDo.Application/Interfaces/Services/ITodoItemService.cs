using ToDo.Application.DTOs.Common;
using ToDo.Application.DTOs.TodoItem;

namespace ToDo.Application.Interfaces.Services
{
    public interface ITodoItemService
    {
        Task<PaginatedResultDto<TodoItemListDto>> GetAllTodoItemsAsync(TodoItemQueryParameters queryParameters, string userId, CancellationToken cancellationToken);
        Task<DetailTodoItemDto> GetTodoItemByIdAsync(Guid id, string userId, CancellationToken cancellationToken);
        Task<DetailTodoItemDto> CreateTodoItemAsync(CreateTodoItemDto createTodoItemDto, string userId, CancellationToken cancellationToken);
        Task<DetailTodoItemDto> UpdateTodoItemAsync(Guid id, UpdateTodoItemDto updateTodoItemDto, string userId, CancellationToken cancellationToken);
        Task DeleteTodoItemAsync(Guid id, string userId, CancellationToken cancellationToken);
    }
}
