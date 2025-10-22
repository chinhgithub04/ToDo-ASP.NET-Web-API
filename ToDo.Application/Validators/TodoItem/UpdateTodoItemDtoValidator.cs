using FluentValidation;
using ToDo.Application.DTOs.TodoItem;

namespace ToDo.Application.Validators.TodoItem
{
    public class UpdateTodoItemDtoValidator : AbstractValidator<UpdateTodoItemDto>
    {
        public UpdateTodoItemDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title cannot be empty.")
                .MaximumLength(500).WithMessage("Title cannot exceed 500 characters.")
                .When(x => x.Title != null);

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.")
                .When(x => x.Description != null);
        }
    }
}
