using AdedonhaAPI.Application.Common.Context;
using Serilog.Context;
using System.Text.RegularExpressions;

namespace AdedonhaAPI.Extensions
{
    public class OperationIdMiddleware
    {
        private const string HeaderName = "X-Operation-Id";
        private static readonly Regex ValidOperationIdPattern = new("^[A-Za-z0-9._-]{1,64}$", RegexOptions.Compiled);
        private readonly RequestDelegate _next;

        public OperationIdMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            var incomingValue = context.Request.Headers.TryGetValue(HeaderName, out var incoming) ? incoming.ToString() : null;

            var operationId = incomingValue != null && ValidOperationIdPattern.IsMatch(incomingValue)
                ? incomingValue
                : Guid.NewGuid().ToString();

            OperationContext.Set(operationId);

            context.Response.OnStarting(() =>
            {
                context.Response.Headers[HeaderName] = operationId;
                return Task.CompletedTask;
            });

            using (LogContext.PushProperty("OperationId", operationId))
            {
                try { await _next(context); }
                finally { OperationContext.Clear(); }
            }
        }
    }

    public static class OperationIdMiddlewareExtensions
    {
        public static IApplicationBuilder UseOperationId(this IApplicationBuilder app) =>
            app.UseMiddleware<OperationIdMiddleware>();
    }
}
