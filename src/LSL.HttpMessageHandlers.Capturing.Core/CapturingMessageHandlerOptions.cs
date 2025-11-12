using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LSL.HttpMessageHandlers.Capturing.Core.Infrastructure;

namespace LSL.HttpMessageHandlers.Capturing.Core;

/// <summary>
/// Capturing message handler options
/// </summary>
public sealed class CapturingMessageHandlerOptions
{
    internal List<Func<IServiceProvider, IAsyncRequestAndResponseCapturer>> HandlerFactories { get; } = [];

    /// <summary>
    /// Adds a capturing handler factory
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public CapturingMessageHandlerOptions AddCapturingHandlerFactory(Func<IServiceProvider, IAsyncRequestAndResponseCapturer> factory, int? index = null) =>
        index switch
        {
            int intIndex => ReturnThis(() => HandlerFactories.Insert(intIndex, factory.AssertNotNull(nameof(factory)))),
            null => ReturnThis(() => HandlerFactories.Add(factory.AssertNotNull(nameof(factory))))
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private CapturingMessageHandlerOptions ReturnThis(Action action)
    {
        action();
        return this;
    }
}
