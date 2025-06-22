using Microsoft.Extensions.Caching.Memory;
using MinimalApis.Model;
using System.Text.Json;

namespace MinimalApis.Middlewares;

public class IdempotencyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _memoryCache;
    public IdempotencyMiddleware(RequestDelegate next, IMemoryCache memoryCache)
    {
        _next = next;
        _memoryCache = memoryCache;
    }

    public async Task Invoke(HttpContext context, IServiceProvider serviceProvider)
    {
        if(context.Request.Method != HttpMethod.Post.Method)
        {
            await _next(context);
        }

        _ = context.Request.Headers.TryGetValue("Idempotency-Key", out var idempotencyKey);
        if (string.IsNullOrEmpty(idempotencyKey))
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 400;
            var error = ResultModel<string>.Error(400, $"Required `Idempotency-Key`");
            await context.Response.WriteAsync(JsonSerializer.Serialize(error));
            return;
        }

        var existing = _memoryCache.Get(idempotencyKey.ToString());
        if (existing is not null)
        {
            context.Response.ContentType = "application/json";
            var response = ResultModel<string>.Success($"Invalid `Idempotency-Key`: {idempotencyKey}");
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            return;
        }

        var originalBody = context.Response.Body;
        using var ms = new MemoryStream();
        context.Response.Body = ms;

        await _next(context);

        ms.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(ms).ReadToEndAsync();
        _memoryCache.Set(idempotencyKey.ToString(), responseBody);
        ms.Seek(0, SeekOrigin.Begin);
        await ms.CopyToAsync(originalBody);
    }
}
