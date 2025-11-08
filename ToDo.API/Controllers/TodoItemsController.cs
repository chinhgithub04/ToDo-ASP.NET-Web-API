using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDo.Application.DTOs.Common;
using ToDo.Application.DTOs.TodoItem;
using ToDo.Application.Interfaces.Services;

namespace ToDo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TodoItemsController : BaseApiController
    {
        private readonly ITodoItemService _todoItemService;
        public TodoItemsController(ITodoItemService todoItemService)
        {
            _todoItemService = todoItemService;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto<PaginatedResultDto<TodoItemDto>>>> GetAllTodoItems([FromQuery] TodoItemQueryParameters queryParameters, CancellationToken cancellationToken = default)
        {
            var userId = GetUserIdFromClaims();
            var paginatedTodoItem = await _todoItemService.GetAllTodoItemsAsync(queryParameters, userId, cancellationToken);

            return OkResponse(paginatedTodoItem, "Todo items retrieved successfully");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto<DetailTodoItemDto>>> GetTodoItemById(Guid id, CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromClaims();
            var detailTodoItemDto = await _todoItemService.GetTodoItemByIdAsync(id, userId, cancellationToken);

            return OkResponse(detailTodoItemDto, "Todo item retrieved successfully");
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDto<DetailTodoItemDto>>> CreateTodoItem([FromBody] CreateTodoItemDto createTodoItemDto, CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromClaims();
            var detailTodoItemDto = await _todoItemService.CreateTodoItemAsync(createTodoItemDto, userId, cancellationToken);

            var response = new ResponseDto<DetailTodoItemDto>
            {
                Data = detailTodoItemDto,
                IsSuccess = true,
                Message = "Todo item created successfully",
                StatusCode = 201
            };

            return CreatedAtAction(nameof(GetTodoItemById), new { id = detailTodoItemDto.Id }, response);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<ResponseDto<DetailTodoItemDto>>> UpdateTodoItem(Guid id, [FromBody] UpdateTodoItemDto updateTodoItemDto, CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromClaims();
            var detailTodoItemDto = await _todoItemService.UpdateTodoItemAsync(id, updateTodoItemDto, userId, cancellationToken);

            return OkResponse(detailTodoItemDto, "Todo item updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTodoItem(Guid id, CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromClaims();
            await _todoItemService.DeleteTodoItemAsync(id, userId, cancellationToken);

            return NoContent();
        }
    }
}