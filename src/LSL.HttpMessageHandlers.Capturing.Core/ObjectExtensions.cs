using System;
using LSL.HttpMessageHandlers.Capturing.Core.Infrastructure;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;

internal static class ObjectExtensions
{
    public static T With<T>(this T source, Action<T> configurator)
    {
        configurator.AssertNotNull(nameof(configurator)).Invoke(source.AssertNotNull(nameof(source)));
        return source;
    }
}

