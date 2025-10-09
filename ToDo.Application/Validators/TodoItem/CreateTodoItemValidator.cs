using FluentValidation;
using ToDo.Application.DTOs.TodoItem;
using ToDo.Application.Interfaces.Services;

namespace ToDo.Application.Validators.TodoItem
{
    public class CreateTodoItemValidator : AbstractValidator<CreateTodoItemDto>
    {
        private readonly ICategoryService _categoryService;

        public CreateTodoItemValidator(ICategoryService categoryService)
        {
            _categoryService = categoryService;

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title cannot be empty.")
                .MaximumLength(500).WithMessage("Title cannot exceed 500 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.")
                .When(x => x.Description != null);

            RuleFor(x => x.CategoryId)
                .MustAsync(CategoryExists).WithMessage("Selected category does not exist.")
                .When(x => x.CategoryId.HasValue);
        }

        private async Task<bool> CategoryExists(Guid? categoryId, CancellationToken cancellationToken)
        {
            if (!categoryId.HasValue)
                return true;

            return await _categoryService.CategoryExistsAsync(categoryId.Value, cancellationToken);
        }
    }
}