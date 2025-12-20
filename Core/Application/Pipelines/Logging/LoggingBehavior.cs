using Core.CrossCuttingConcerns.Logging;
using Core.CrossCuttingConcerns.Logging.Abstraction;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Core.Application.Pipelines.Logging;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>, ILoggableRequest
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger _logger;

    public LoggingBehavior(IHttpContextAccessor httpContextAccessor, ILogger logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var userName = httpContext?.User?.Identity?.Name ?? "Anonymous";

        List<LogParameter> logParameters = [new LogParameter { Type = request.GetType().Name, Value = request }];
        LogDetail logDetail = new()
        {
            MethodName=next.Method.Name,
            Parameters = logParameters,
            User = userName ?? "Anonymous"
        };
        try
        {
            _logger.Information(JsonSerializer.Serialize(logDetail));
            return await next();
        }
        catch (Exception ex)
        {
            LogDetailWithException logDetailWithException = new() {ExceptionMessage=ex.Message,FullName=userName!,MethodName=next.Method.Name,Parameters= logParameters,User=userName!};
            _logger.Error(JsonSerializer.Serialize(logDetailWithException));
            throw;
        }
    }
}
