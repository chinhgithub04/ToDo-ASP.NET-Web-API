using FluentValidation;
using ToDo.Application.DTOs.TodoItem;
using ToDo.Application.Validators.Common;

namespace ToDo.Application.Validators.TodoItem
{
    public class TodoItemQueryParametersValidator : AbstractValidator<TodoItemQueryParameters>
    {
        public TodoItemQueryParametersValidator()
        {
            Include(new PaginationQueryValidator());

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
    }
}
