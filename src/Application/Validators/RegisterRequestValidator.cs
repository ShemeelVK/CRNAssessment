using CRNAssessment.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CRNAssessment.Application.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters.")
                .Matches("^[a-zA-Z0-9]*$").WithMessage("Username can only contain letters and numbers (no special characters).");

            RuleFor(x => x.Password)
                 .NotEmpty().WithMessage("Password is required.")
                 .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
                 .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                 .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                 .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                 .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
        }
    }
}
