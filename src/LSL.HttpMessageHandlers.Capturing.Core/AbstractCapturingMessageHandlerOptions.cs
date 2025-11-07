using System;
using System.Collections.Generic;
using LSL.HttpMessageHandlers.Capturing.Core.Infrastructure;

namespace LSL.HttpMessageHandlers.Capturing.Core;

/// <summary>
/// AbstractCapturingMessageHandlerOptions
/// </summary>
/// <typeparam name="TFactory"></typeparam>
/// <typeparam name="TSelf"></typeparam>
public abstract class AbstractCapturingMessageHandlerOptions<TFactory, TSelf>
    where TSelf : AbstractCapturingMessageHandlerOptions<TFactory, TSelf>
{
    internal TSelf Self { get; set; } = default!;

    internal List<TFactory> Factories { get; set; } = [];

    /// <summary>
    /// Adds a capturing handler factory
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public TSelf AddCapturingHandlerFactory(TFactory factory, int? index = null) =>
        index switch
        {
            int intIndex => ReturnThis(() => Factories.Insert(intIndex, factory.AssertNotNull(nameof(factory)))),
            null => ReturnThis(() => Factories.Add(factory.AssertNotNull(nameof(factory))))
        };

    private TSelf ReturnThis(Action action)
    {
        action();
        return Self;
    }
}