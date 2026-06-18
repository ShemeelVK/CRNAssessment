using CRNAssessment.Application.DTOs;
using FluentValidation;

namespace CRNAssessment.Application.Validators;

public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("A valid Product ID is required.");

        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("Product Name is required.")
            .MaximumLength(255).WithMessage("Product Name cannot exceed 255 characters.");

        RuleForEach(x => x.Items).SetValidator(new UpdateItemValidator());
    }
}

public class UpdateItemValidator : AbstractValidator<UpdateItemDto>
{
    public UpdateItemValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Item ID is required for updates.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative.");
    }
}
