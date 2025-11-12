using System;
using LSL.HttpMessageHandlers.Capturing.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LSL.HttpMessageHandlers.Capturing.Core;

/// <summary>
/// Is enabled capturing handler extensions
/// </summary>
public static class IsEnabledCapturingHandlerExtensions
{
    /// <summary>
    /// Adds a request and response handler that will stop further processing if <see cref="IsEnabledProviderOptions.IsEnabled"/>
    /// returns <see langword="false"/>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="configurator"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static ICapturingHandlerBuilder AddIsEnabledProvider(
        this ICapturingHandlerBuilder source,
        Action<IsEnabledProviderOptions>? configurator = null,
        int? index = null)
    {
        var name = source.BuildUniqueName();

        source.Services
            .Configure<IsEnabledProviderOptions>(name, (c) =>
            {
                c.IsEnabled = "true".Equals(
                    Environment.GetEnvironmentVariable("CAPTURING_MESSAGE_HANDLERS_ENABLED"),
                    StringComparison.InvariantCultureIgnoreCase);

                configurator?.Invoke(c);
            });

        return source.AddCapturingHandlerFactory(
            sp => new IsEnabledCapturingHandler(
                new IsEnabledOptionsContainer(name, sp.GetRequiredService<IOptionsMonitor<IsEnabledProviderOptions>>())
            ),
            index
        );
    }

    /// <summary>
    /// Adds a request and response handler that will stop further processing 
    /// if provided <see cref="IIsEnabledProvider"/> implementation's
    /// <see cref="IIsEnabledProvider.IsEnabled"/> property 
    /// returns <see langword="false"/>
    /// </summary>
    /// <typeparam name="TProvider"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static ICapturingHandlerBuilder AddIsEnabledProvider<TProvider>(this ICapturingHandlerBuilder source)
        where TProvider : class, IIsEnabledProvider
    {
        source.AddCapturingHandlerFactory(sp => new IsEnabledCapturingHandler(sp.GetRequiredService<TProvider>()))
            .Services
            .FluentlyTryAddTransient<TProvider>();

        return source;
    }
}