using Core.CrossCuttingConcerns.Expeptions.Types;
using FluentValidation;
using FluentValidation.Results;
using ValidationException = Core.CrossCuttingConcerns.Expeptions.Types.ValidationException;

namespace Core.Application.Pipelines.Validation;

public static class ValidationTool
{
    public static async Task Validate<T>(IValidator<T> validator,T entity,CancellationToken cancellationToken)
    {
        ValidationContext<T> context = new(entity);
        ValidationResult result = await validator.ValidateAsync(context,cancellationToken);
        if (!result.IsValid)
        {
            var errors = result.Errors
               .GroupBy(
                    keySelector: p => p.PropertyName,
                    resultSelector: 
                    (propertName, errors) => new ValidationExceptionModel { Property = propertName, Errors = errors.Select(e => e.ErrorMessage) })
               .ToList();
        }
            throw new ValidationException(result.Errors.ToString());
    }
}
