using FluentValidation;
using ToDo.Application.DTOs.Category;

namespace ToDo.Application.Validators.Category
{
    public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
    {
        public UpdateCategoryDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name cannot be empty.")
                .MaximumLength(60).WithMessage("Name cannot exceed 60 characters.")
                .When(x => x.Name != null);

            RuleFor(x => x.Color)
                .NotEmpty().WithMessage("Color cannot be empty.")
                .MaximumLength(7).WithMessage("Color cannot exceed 7 characters.")
                .Matches(@"^#([A-Fa-f0-9]{6})$").WithMessage("Color must be a valid 6-digit hex code (e.g. #FFFFFF).")
                .When(x => x.Color != null);
        }
    }
}