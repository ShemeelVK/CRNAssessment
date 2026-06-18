using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using CRNAssessment.Application.DTOs;

namespace CRNAssessment.Application.Validators
{
    public class CreateProductValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("Product Name is required")
                .MaximumLength(255).WithMessage("Product name cannot exceed 255 characters");

            RuleForEach(x => x.Items).SetValidator(new CreateItemValidator());

        }
    }

    public class CreateItemValidator : AbstractValidator<CreateItemDto>
    {
        public CreateItemValidator()
        {
            RuleFor(x => x.Quantity)
           .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative.");
        }
    }
}
