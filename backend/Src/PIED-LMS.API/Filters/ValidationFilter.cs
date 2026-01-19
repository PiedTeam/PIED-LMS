namespace PIED_LMS.API.Filters;

public sealed class ValidationFilter(IServiceProvider serviceProvider) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var parameter in context.ActionDescriptor.Parameters)
        {
            var declaredType = parameter.ParameterType;
            var validatorType = typeof(IValidator<>).MakeGenericType(declaredType);
            var validator = serviceProvider.GetService(validatorType) as IValidator;

            if (validator is not null)
            {
                context.ActionArguments.TryGetValue(parameter.Name!, out var value);
                var validationContext = new ValidationContext<object>(value!);
                var validationResult = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        );

                    context.Result = new BadRequestObjectResult(new
                    {
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                        Title = "One or more validation errors occurred.",
                        Status = 400,
                        Errors = errors
                    });
                    return;
                }
            }
        }

        await next();
    }
}
