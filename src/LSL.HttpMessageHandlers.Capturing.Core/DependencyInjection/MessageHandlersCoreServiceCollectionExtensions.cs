using System;
using LSL.HttpMessageHandlers.Capturing.Core.DependencyInjection;
using LSL.HttpMessageHandlers.Capturing.Core.Infrastructure;
using LSL.HttpMessageHandlers.Capturing.Core;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Message handlers core service collection extensions
/// </summary>
public static class MessageHandlersCoreServiceCollectionExtensions
{
    /// <summary>
    /// Adds request and response capturing to the client
    /// </summary>
    /// <param name="source"></param>
    /// <param name="configurator"></param>
    /// <returns></returns>
    public static IHttpClientBuilder AddRequestAndResponseCapturing(this IHttpClientBuilder source, Action<ICapturingHandlerBuilder>? configurator = null)
    {
        var builder = new CapturingHandlerBuilder(source.Name, source.Services);

        source.AssertNotNull(nameof(source)).Services
            .FluentlyTryAddTransient<CapturingMessageHandler>()
            .FluentlyTryAddSingleton<IExecutorBuilder, ExecutorBuilder>();

        configurator?.Invoke(builder);

        return source.AddHttpMessageHandler(
            sp => sp.GetRequiredService<CapturingMessageHandler>().With(
                a => a.Name = builder.Name
            )
        );
    }

    /// <summary>
    /// Configure all named options for request and response capturing
    /// </summary>
    /// <param name="source"></param>
    /// <param name="configurator"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureAllRequestAndResponseCapturing(this IServiceCollection source, Action<ICapturingHandlerBuilder> configurator)
    {
        var builder = new CapturingHandlerBuilder(null, source.AssertNotNull(nameof(source)));
        configurator.AssertNotNull(nameof(configurator)).Invoke(builder);
        return source;
    }
}