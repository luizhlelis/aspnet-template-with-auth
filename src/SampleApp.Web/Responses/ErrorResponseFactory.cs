using System.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SampleApp.Application.Domain;

namespace SampleApp.Web.Responses;

public class ErrorMessage
{
    public string Msg { get; set; }
}

public interface IErrorResponse
{
    public string Type { get; }

    public string Title { get; }

    public int Status { get; }

    public string TraceId { get; }

    public ErrorMessage Error { get; }
}

public class ForbiddenResponse : IErrorResponse
{
    public ForbiddenResponse(string message, string traceparent)
    {
        TraceId = traceparent;
        Error = new ErrorMessage { Msg = message };
    }

    public string Type => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3";

    public string Title => ErrorCode.Forbidden;

    public int Status => 403;

    public string TraceId { get; set; }

    public ErrorMessage Error { get; set; }
}

public class NotFoundResponse : IErrorResponse
{
    public NotFoundResponse(string message, string traceparent)
    {
        TraceId = traceparent;
        Error = new ErrorMessage { Msg = message };
    }

    public string Type => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4";

    public string Title => "NOT_FOUND_ERROR";

    public int Status => 404;

    public string TraceId { get; set; }

    public ErrorMessage Error { get; set; }
}

public class BadRequestResponse : IErrorResponse
{
    public BadRequestResponse(string message, string traceparent)
    {
        TraceId = traceparent;
        Error = new ErrorMessage { Msg = message };
    }

    public string Type => "https://tools.ietf.org/html/rfc7231#section-6.5.1";

    public string Title => "BAD_REQUEST_ERROR";

    public int Status => 400;

    public string TraceId { get; set; }

    public ErrorMessage Error { get; set; }
}

public static class ErrorResponseFactory
{
    public static IErrorResponse CreateErrorResponse(ModelStateDictionary modelState,
        string traceparent)
    {
        if (modelState[ErrorCode.Forbidden] is not null)
            return new ForbiddenResponse(
                modelState[ErrorCode.Forbidden]?.Errors.FirstOrDefault()?.ErrorMessage ??
                String.Empty, traceparent);

        return new NotFoundResponse(
            modelState[ErrorCode.NotFound]?.Errors.FirstOrDefault()?.ErrorMessage ??
            String.Empty,
            traceparent);
    }

    public static HttpStatusCode GetResponseStatusCode(ModelStateDictionary modelState)
    {
        if (modelState[ErrorCode.Forbidden] is not null)
            return HttpStatusCode.Forbidden;

        return modelState[ErrorCode.NotFound] is not null
            ? HttpStatusCode.NotFound
            : HttpStatusCode.BadRequest;
    }
}
