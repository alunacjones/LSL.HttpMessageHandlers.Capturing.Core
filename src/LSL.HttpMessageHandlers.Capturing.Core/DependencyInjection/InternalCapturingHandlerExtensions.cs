using System;
using Microsoft.Extensions.DependencyInjection;

namespace LSL.HttpMessageHandlers.Capturing.Core.DependencyInjection;

internal static class InternalCapturingHandlerExtensions
{
    public static ICapturingHandlerBuilder Configure<TOptions>(this ICapturingHandlerBuilder source, string name, Action<TOptions> configurator)
        where TOptions : class
    {
        source.Services.Configure(name, configurator);
        return source;
    }

    public static string BuildUniqueName(this ICapturingHandlerBuilder source) => BuildUniqueName(source.Name);

    public static string BuildUniqueName(string? originalName) => originalName is null ? null! : $"originalName-{Guid.NewGuid()}";
}