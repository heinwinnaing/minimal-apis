using Microsoft.Extensions.Caching.Memory;
using MinimalApis.Model;
using System.Text.Json;

namespace MinimalApis.Filters;

public class IdempotencyFilter : IEndpointFilter
{
    private readonly IMemoryCache _memoryCache;
    public IdempotencyFilter(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;
        var request = httpContext.Request;

        _ = request.Headers.TryGetValue("Idempotency-Key", out var idempotencyKey);
        if (string.IsNullOrEmpty(idempotencyKey))
        {
            var error = ResultModel<string>.Error(400, $"Required `Idempotency-Key` in header");
            return Results.Json(error, contentType: "application/json", statusCode: 400);
        }

        if (_memoryCache.TryGetValue($"{idempotencyKey}", out var cachedResult))
        {
            var error = ResultModel<string>.Success($"Invalid `Idempotency-Key`: {idempotencyKey}");
            //var response = JsonSerializer.Deserialize<ResultModel<object>>($"{cachedResult}");
            return Results.Json(error, contentType: "application/json", statusCode: 400);
        }

        var result = await next(context);

        try 
        {
            if (result is IValueHttpResult res)
            {
                if (res?.Value is not null)
                {
                    _memoryCache.Set(
                        key: idempotencyKey.ToString(),
                        value: JsonSerializer.Serialize(res.Value),
                        absoluteExpiration: DateTime.UtcNow.AddSeconds(60));
                }
            }
        }
        catch (Exception ex) 
        {
            Console.Error.WriteLine(ex);
        }

        return result;
    }
}
