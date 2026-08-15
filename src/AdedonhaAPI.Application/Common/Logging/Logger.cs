using Microsoft.Extensions.Logging;

namespace AdedonhaAPI.Application.Common.Logging
{
    public static class Logger
    {
        public static void LogInfo(this ILogger logger, string message, Context.IRequestContext ctx, Dictionary<string, object?>? dic = null)
        {
            var (template, args) = BuildLine(message, ctx, dic);
            logger.LogInformation(template, args);
        }

        public static void LogWarning(this ILogger logger, string message, Context.IRequestContext ctx, Dictionary<string, object?>? dic = null)
        {
            var (template, args) = BuildLine(message, ctx, dic);
            logger.LogWarning(template, args);
        }

        public static void LogError(this ILogger logger, string message, Context.IRequestContext ctx, Exception ex, Dictionary<string, object?>? dic = null)
        {
            var (template, args) = BuildLine(message, ctx, dic);
            logger.LogError(ex, template, args);
        }

        public static void LogBegin(this ILogger logger, string message, Context.IRequestContext ctx, Dictionary<string, object?>? dic = null)
        {
            var (template, args) = BuildLine($"Início - {message}", ctx, dic);
            logger.LogInformation(template, args);
        }

        public static void LogEnd(this ILogger logger, string message, Context.IRequestContext ctx, Dictionary<string, object?>? dic = null)
        {
            var (template, args) = BuildLine($"Fim - {message}", ctx, dic);
            logger.LogInformation(template, args);
        }

        private static (string Template, object?[] Args) BuildLine(string message, Context.IRequestContext ctx, Dictionary<string, object?>? dic) =>
            dic is null || dic.Count == 0
                ? ("[{origem}] [Usuario: {usuario}] - {message}",
                    new object?[] { ctx.Origem, ctx.UsuarioId, message })
                : ("[{origem}] [Usuario: {usuario}] - {message} {@dic}",
                    new object?[] { ctx.Origem, ctx.UsuarioId, message, dic });
    }
}
