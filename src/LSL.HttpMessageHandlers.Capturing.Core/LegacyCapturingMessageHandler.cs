using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace LSL.HttpMessageHandlers.Capturing.Core;

/// <summary>
/// Legacy capturing message handler
/// </summary>
public class LegacyCapturingMessageHandler : AbstractCapturingMessageHandler
{
    private readonly LegacyCapturingMessageHandlerOptions _options;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="options"></param>
    /// <param name="httpMessageHandler"></param>
    public LegacyCapturingMessageHandler(LegacyCapturingMessageHandlerOptions options, HttpMessageHandler? httpMessageHandler = null) : base(ExecutorBuilder.Instance)
    {
        InnerHandler = httpMessageHandler ?? new HttpClientHandler();
        _options = options;
    }

    /// <inheritdoc/>
    protected override IEnumerable<Func<IAsyncRequestAndResponseCapturer>> GetFactories() => _options.Factories;
}