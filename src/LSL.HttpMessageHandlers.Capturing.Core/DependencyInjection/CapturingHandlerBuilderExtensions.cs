using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace LSL.HttpMessageHandlers.Capturing.Core.DependencyInjection;

/// <summary>
/// Capturing handler builder extensions
/// </summary>
public static class CapturingHandlerBuilderExtensions
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
        var name = $"{source.Name}-{Guid.NewGuid()}";

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

    /// <summary>
    /// Adds a capturing handler factory method
    /// </summary>
    /// <param name="source"></param>
    /// <param name="factory"></param>
    /// <param name="index"></param>
    /// <returns></returns>    
    public static ICapturingHandlerBuilder AddCapturingHandlerFactory(
        this ICapturingHandlerBuilder source,
        Func<IServiceProvider, IAsyncRequestAndResponseCapturer> factory,
        int? index = null
    ) =>
    source.Configure<CapturingMessageHandlerOptions>(source.Name, c => c.AddCapturingHandlerFactory(factory, index));

    /// <summary>
    /// Adds a capturing delegate
    /// </summary>
    /// <param name="source"></param>
    /// <param name="delegate"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static ICapturingHandlerBuilder AddCapturingHandlerDelegate(
        this ICapturingHandlerBuilder source,
        Func<CaptureContext, Task> @delegate,
        int? index = null
    ) =>
    source.Configure<CapturingMessageHandlerOptions>(source.Name, c => c.AddCapturingHandlerFactory(_ =>
        new DelegatingAsyncRequestAndResponseCapturer(@delegate),
        index
    ));
}
