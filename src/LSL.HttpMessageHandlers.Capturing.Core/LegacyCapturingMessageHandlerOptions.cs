using System;

namespace LSL.HttpMessageHandlers.Capturing.Core;

/// <summary>
/// Legacy capturing message handler options
/// </summary>
public sealed class LegacyCapturingMessageHandlerOptions :
    AbstractCapturingMessageHandlerOptions<Func<IAsyncRequestAndResponseCapturer>, LegacyCapturingMessageHandlerOptions>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public LegacyCapturingMessageHandlerOptions()
    {
        Self = this;
    }
}
