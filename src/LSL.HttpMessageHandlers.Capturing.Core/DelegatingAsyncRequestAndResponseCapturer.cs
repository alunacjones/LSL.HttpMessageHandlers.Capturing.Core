using System;
using System.Threading.Tasks;

namespace LSL.HttpMessageHandlers.Capturing.Core;

/// <summary>
/// Delegating async request and response capturer
/// </summary>
/// <param name="delegate"></param>
public class DelegatingAsyncRequestAndResponseCapturer(Func<CaptureContext, Task> @delegate) : IAsyncRequestAndResponseCapturer
{
    /// <inheritdoc/>
    public Task CaptureAsync(CaptureContext context) => @delegate(context);
}