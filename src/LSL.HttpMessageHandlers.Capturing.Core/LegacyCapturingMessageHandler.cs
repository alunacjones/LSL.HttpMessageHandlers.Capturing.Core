using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace LSL.HttpMessageHandlers.Capturing.Core;

/// <summary>
/// Legacy capturing message handler
/// </summary>
public class LegacyCapturingMessageHandler(LegacyCapturingMessageHandlerOptions options)
    : AbstractCapturingMessageHandler(ExecutorBuilder.Instance)
{
    /// <inheritdoc/>
    protected override IEnumerable<Func<IAsyncRequestAndResponseCapturer>> GetFactories() => options.Factories;
}