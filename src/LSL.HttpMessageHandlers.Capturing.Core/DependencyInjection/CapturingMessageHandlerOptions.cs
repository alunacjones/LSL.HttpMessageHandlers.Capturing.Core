using System;

namespace LSL.HttpMessageHandlers.Capturing.Core.DependencyInjection;

/// <summary>
/// Capturing message handler options
/// </summary>
public sealed class CapturingMessageHandlerOptions
    : AbstractCapturingMessageHandlerOptions<Func<IServiceProvider, IAsyncRequestAndResponseCapturer>, CapturingMessageHandlerOptions>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public CapturingMessageHandlerOptions()
    {
        Self = this;
    }
}
