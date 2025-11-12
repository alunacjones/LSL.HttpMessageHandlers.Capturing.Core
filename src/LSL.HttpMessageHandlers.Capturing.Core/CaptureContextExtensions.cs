using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LSL.HttpMessageHandlers.Capturing.Core;

/// <summary>
/// Capture context extensions
/// </summary>
public static class CaptureContextExtensions
{
    /// <summary>
    /// Run an async handler when a send exception has been captured
    /// </summary>
    /// <param name="source"></param>
    /// <param name="asyncExceptionHandler"></param>
    /// <returns></returns>
    public static Task WithExceptionAndRequestAsync(this CaptureContext source, Func<Exception, HttpRequestMessage, Task> asyncExceptionHandler) =>
        source.SendException is null
            ? Task.CompletedTask
            : asyncExceptionHandler(source.SendException, source.Request);

    /// <summary>
    /// Run a sync handler when a send exception has been captured
    /// </summary>
    /// <param name="source"></param>
    /// <param name="exceptionHandler"></param>
    public static void WithExceptionAndRequest(this CaptureContext source, Action<Exception, HttpRequestMessage> exceptionHandler) =>
        source.WithExceptionAndRequestAsync((exception, request) =>
        {
            exceptionHandler(exception, request);
            return Task.CompletedTask;
        }).GetAwaiter().GetResult();

    /// <summary>
    /// Run an async handler when a response has been captured i.e. no send exception occurred
    /// </summary>
    /// <param name="source"></param>
    /// <param name="asyncRequestAndResponseHandler"></param>
    /// <returns></returns>
    public static Task WithRequestAndResponseAsync(
        this CaptureContext source,
        Func<HttpRequestMessage, HttpResponseMessage, Task> asyncRequestAndResponseHandler) =>
        source.Response is null
            ? Task.CompletedTask
            : asyncRequestAndResponseHandler(source.Request, source.Response);

    /// <summary>
    /// Runs a sync handler when a response has been captured i.e. no send exception occurred
    /// </summary>
    /// <param name="source"></param>
    /// <param name="requestAndResponseHandler"></param>
    public static void WithRequestAndResponse(
        this CaptureContext source,
        Action<HttpRequestMessage, HttpResponseMessage> requestAndResponseHandler) =>
        source.WithRequestAndResponseAsync((request, response) =>
        {
            requestAndResponseHandler(request, response);
            return Task.CompletedTask;
        }).GetAwaiter().GetResult();
}