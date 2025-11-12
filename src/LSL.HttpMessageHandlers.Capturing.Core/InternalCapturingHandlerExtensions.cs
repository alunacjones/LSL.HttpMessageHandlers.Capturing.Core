using System;
using LSL.HttpMessageHandlers.Capturing.Core;
using Microsoft.Extensions.DependencyInjection;

namespace LSL.HttpMessageHandlers.Capturing.Core;

internal static class InternalCapturingHandlerExtensions
{
    public static ICapturingHandlerBuilder Configure<TOptions>(this ICapturingHandlerBuilder source, string name, Action<TOptions> configurator)
        where TOptions : class
    {
        source.Services.Configure(name, configurator);
        return source;
    }
}