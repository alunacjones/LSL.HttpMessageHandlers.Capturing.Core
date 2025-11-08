using System;
using LSL.HttpMessageHandlers.Capturing.Core.Infrastructure;

namespace LSL.HttpMessageHandlers.Capturing.Core;

internal static class ObjectExtensions
{
    public static T With<T>(this T source, Action<T> configurator)
    {
        configurator.AssertNotNull(nameof(configurator)).Invoke(source.AssertNotNull(nameof(source)));
        return source;
    }
}

