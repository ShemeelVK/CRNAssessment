using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRNAssessment.API.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //intercept the incoming arguments 
            var actionArguments = context.ActionArguments.Values;

            foreach (var argument in actionArguments)
            {
                if (argument == null) continue;

                var argumentType = argument.GetType();

                //ask dependency injection if a FluentValidator exists for this specific DTO type
                var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);
                var validator = context.HttpContext.RequestServices.GetService(validatorType) as IValidator;

                // if a validator is found, execute its rules against the DTO
                if (validator != null)
                {
                    var validationContext = new ValidationContext<object>(argument);
                    var validationResult = await validator.ValidateAsync(validationContext);

                    // if the data breaks the rules, immediately stop the request and return a 400 Bad Request
                    if (!validationResult.IsValid)
                    {
                        var errors = validationResult.Errors
                            .Select(e => new { Field = e.PropertyName, Error = e.ErrorMessage })
                            .ToList();

                        context.Result = new BadRequestObjectResult(new
                        {
                            Message = "Validation failed",
                            Errors = errors
                        });

                        return; //the request is blocked and never reaches the Controller.
                    }
                }
            }

            //if everything passed, or if no validator was needed, allow request to proceed to the Controller
            await next();
        }
    }
}
