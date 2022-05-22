using System.Diagnostics;
using Newtonsoft.Json;

namespace SampleApp.Web.Responses;

public static class ExceptionHandler
{
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
    {
        builder.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var traceparent =
                    Activity.Current?.Id ?? context.Request.HttpContext.TraceIdentifier;

                var errorResponse = new
                {
                    error = new
                    {
                        msg =
                            $"Oh, sorry! We didn't expect that 😬! Please, inform the ID {traceparent} so we can help you properly."
                    },
                    traceId = traceparent,
                    type = "UNEXPECTED_ERROR"
                };

                var responseBody = JsonConvert.SerializeObject(errorResponse);
                await context.Response.WriteAsync(responseBody);
            });
        });

        return builder;
    }
}
