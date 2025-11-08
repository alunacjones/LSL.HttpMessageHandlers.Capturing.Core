using System;
using System.Net.Http;
namespace LSL.HttpMessageHandlers.Capturing.Core;

/// <summary>
/// The capture context
/// </summary>
/// <param name="Request"></param>
/// <param name="Response"></param>
/// <param name="SendException"></param>
public sealed record CaptureContext(HttpRequestMessage Request, HttpResponseMessage? Response, Exception? SendException)
{
    internal bool ShouldStop { get; private set; } = false;

    /// <summary>
    /// Stops further capturing handlers from running
    /// </summary>
    public void StopProcessing(bool shouldStopProcessing = true) => ShouldStop = shouldStopProcessing;

    /// <summary>
    /// Is true when an exception was captured on sending. The exception will still propagate
    /// but a capturing handler could do something with the exception prior to re-throw
    /// </summary>
    public bool SendExceptionThrown => SendException is not null;
};

