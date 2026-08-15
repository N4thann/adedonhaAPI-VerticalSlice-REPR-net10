using AdedonhaAPI.Application.Common.Context;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace AdedonhaAPI.Extensions
{
    public static class ApiExceptionMiddleware
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            var isDevelopment = app.ApplicationServices.GetRequiredService<IHostEnvironment>().IsDevelopment();

            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        await context.Response.WriteAsync(new ErrorDetailsOutput
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = contextFeature.Error.Message,
                            Trace = isDevelopment ? contextFeature.Error.StackTrace : null,
                            OperationId = OperationContext.Current ?? "unknown",
                        }.ToString());
                    }
                });
            });
        }
    }
}
