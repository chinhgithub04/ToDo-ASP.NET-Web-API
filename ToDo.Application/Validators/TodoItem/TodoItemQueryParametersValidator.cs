using FluentValidation;
using ToDo.Application.DTOs.TodoItem;
using ToDo.Application.Interfaces.Services;

namespace ToDo.Application.Validators.TodoItem
{
    public class TodoItemQueryParametersValidator : AbstractValidator<TodoItemQueryParameters>
    {
        private readonly ICategoryService _categoryService;

        public TodoItemQueryParametersValidator(ICategoryService categoryService)
        {
            _categoryService = categoryService;

            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page number must be at least 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("Page size must be between 1 and 100.");

            RuleFor(x => x.CategoryId)
                .MustAsync(CategoryExists).WithMessage("Selected category does not exist.")
                .When(x => x.CategoryId.HasValue);

            RuleFor(x => x.SearchTerm)
                .MaximumLength(500)
                .WithMessage("Search term cannot exceed 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.SearchTerm));

            RuleFor(x => x.SortBy)
                .IsInEnum()
                .WithMessage("Invalid value for SortBy.")
                .When(x => x.SortBy.HasValue);

            RuleFor(x => x.SortOrder)
                .IsInEnum()
                .WithMessage("Invalid value for SortOrder.")
                .When(x => x.SortOrder.HasValue);
        }

        private async Task<bool> CategoryExists(Guid? categoryId, CancellationToken cancellationToken)
        {
            if (!categoryId.HasValue)
                return true;

            return await _categoryService.CategoryExistsAsync(categoryId.Value, cancellationToken);
        }
    }
}
