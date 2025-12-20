using Core.CrossCuttingConcerns.Expeptions.Types;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidationException = Core.CrossCuttingConcerns.Expeptions.Types.ValidationException;

namespace Core.Application.Pipelines.Validation;

public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ValidationContext<object> context = new ValidationContext<object>(request);
        var errors = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .GroupBy(
                f => f.PropertyName,
                (propertyName, failures) => new ValidationExceptionModel
                {
                    Property = propertyName,
                    Errors = failures.Select(e => e.ErrorMessage)
                })
            .ToList();

        if (errors.Any())
            throw new ValidationException(errors);
        TResponse response = await next();
        return response;
    }
}
