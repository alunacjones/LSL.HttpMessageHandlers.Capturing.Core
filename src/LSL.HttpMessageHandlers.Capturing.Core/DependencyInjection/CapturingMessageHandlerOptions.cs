using System;
using System.Collections.Generic;
using LSL.HttpMessageHandlers.Capturing.Core.Infrastructure;

namespace LSL.HttpMessageHandlers.Capturing.Core.DependencyInjection;

/// <summary>
/// Capturing message handler options
/// </summary>
public sealed class CapturingMessageHandlerOptions
{
    private readonly List<Func<IServiceProvider, IAsyncRequestAndResponseCapturer>> _factories = [];
    internal List<Func<IServiceProvider, IAsyncRequestAndResponseCapturer>> Factories => _factories;

    /// <summary>
    /// Adds a capturing handler factory
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public CapturingMessageHandlerOptions AddCapturingHandlerFactory(Func<IServiceProvider, IAsyncRequestAndResponseCapturer> factory, int? index = null)
    {
        return index switch
        {
            int intIndex => ReturnThis(() => _factories.Insert(intIndex, factory.AssertNotNull(nameof(factory)))),
            null => ReturnThis(() => _factories.Add(factory.AssertNotNull(nameof(factory))))
        };
    }
    
    private CapturingMessageHandlerOptions ReturnThis(Action action)
    {
        action();
        return this;
    }
}
