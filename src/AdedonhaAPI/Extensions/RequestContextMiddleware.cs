using AdedonhaAPI.Common.Context;
using System.Security.Claims;

namespace AdedonhaAPI.Extensions
{
    public class RequestContextMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestContextMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context, RequestContext requestContext)
        {
            var endpoint = context.GetEndpoint();
            var endpointName = endpoint?.Metadata.GetMetadata<IEndpointNameMetadata>()?.EndpointName;

            requestContext.UsuarioId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            requestContext.NomeUsuario = context.User.FindFirstValue(ClaimTypes.Email);
            requestContext.Origem = endpointName ?? endpoint?.DisplayName ?? context.Request.Path.ToString();

            await _next(context);
        }
    }

    public static class RequestContextMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestContext(this IApplicationBuilder app) =>
            app.UseMiddleware<RequestContextMiddleware>();
    }
}
