using FluentValidation;
using ToDo.Application.DTOs.TodoItem;

namespace ToDo.Application.Validators.TodoItem
{
    public class CreateTodoItemValidator : AbstractValidator<CreateTodoItemDto>
    {
        public CreateTodoItemValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title cannot be empty.")
                .MaximumLength(500).WithMessage("Title cannot exceed 500 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.")
                .When(x => x.Description != null);

            RuleFor(x => x.DueDate)
                .GreaterThanOrEqualTo(DateTimeOffset.UtcNow.AddMinutes(-1)).WithMessage("Due date cannot be in the past.")
                .When(x => x.DueDate.HasValue);

            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage("Invalid value for Priority.");
        }
    }
}