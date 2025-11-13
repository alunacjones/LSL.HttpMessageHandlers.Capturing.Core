using System;
using System.Net.Http;

namespace LSL.HttpMessageHandlers.Capturing.Core;

/// <summary>
/// The capture context
/// </summary>
/// <param name="Request"></param>
/// <param name="Response"></param>
/// <param name="SendException"></param>
/// <param name="TimeToRun"></param>
public sealed record CaptureContext(HttpRequestMessage Request, HttpResponseMessage? Response, Exception? SendException, TimeSpan TimeToRun)
{
    internal bool ShouldStop { get; private set; } = false;

    /// <summary>
    /// Stops further capturing handlers from running
    /// </summary>
    public void StopProcessing(bool shouldStopProcessing = true) => ShouldStop = shouldStopProcessing;
};
